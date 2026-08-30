#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
COI RU Localization Patcher
============================

Патчит русскую локализацию (ru.json) установленных модов Captain of Industry
значениями из твоих собственных файлов-словарей.

Как это работает:
  1. Берём все *.json файлы из DICT_DIR (папка с твоими словарями).
  2. Для каждого словаря ищем папку мода с таким же именем в MODS_DIR
     (сравнение без учёта регистра).
  3. Внутри папки мода рекурсивно ищем все файлы "ru.json".
  4. Если ru.json нет вообще — копируем туда наш словарь как есть
     (см. правило №3 из ТЗ).
  5. Если ru.json есть — проходим по структуре словаря (плоской, вложенной
     или в виде списка пар [["ключ","значение"], ...] — форматы могут
     различаться, идём "путём ключей"), и для каждого ключа, который есть
     и там и там:
       - если это строка с плейсхолдерами ({0}, %s и т.п.) и их количество
         не совпадает с оригиналом — замену пропускаем ("плейсхолдер-мисматч");
       - иначе заменяем значение (даже если оно совпадает с уже имеющимся —
         это отдельно учитывается как "реально изменено" или нет).
  6. Всё, что в нашем словаре есть, а в моде уже нет — "устарело в словаре"
     (мод обновился, ключ пропал).
  7. Всё, что в моде есть, а в нашем словаре нет — "незалокализовано".
  8. Результат сохраняется с отступом в 4 пробела, UTF-8 без BOM.
  9. Перед перезаписью исходный ru.json бэкапится в ru.json.original.json
     (бэкап каждый раз перезаписывается свежим "как было до патча" —
     удобно, чтобы смотреть, что поменялось в моде после апдейта).

В ОТЧЁТЕ по умолчанию в основной таблице показываются только моды, где
есть что заметить (реальные изменения, проблемы, недостающие переводы).
Моды, где всё уже 1-в-1 совпадает с твоим словарём, по умолчанию просто
считаются одной строкой в конце, чтобы не захламлять вывод. Хочешь видеть
их тоже (построчно) — поставь SHOW_CLEAN_MODS = True.

ВАЖНО: этот скрипт нужно запускать заново после каждого обновления игры
или модов — Steam Workshop полностью перезаписывает папки модов, и все
патчи с прошлого раза слетают. Это ожидаемое поведение, не баг.

DICT_DIR указывает на папку "mods (translated)" — только проверенные,
готовые словари. Конфиги ("mods (configs)") и манифесты ("mods (manifests)")
пока не обрабатываются, это отдельная задача на потом.

Пока НЕ обрабатывается:
  - локализация самой игры и патч-нотов (game/ и changelog/).
"""

import json
import re
import shutil
from pathlib import Path

# ============================== НАСТРОЙКИ ==============================

MODS_DIR = Path(r"C:\Users\www\AppData\Roaming\Captain of Industry\Mods")
DICT_DIR = Path(r"C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods (translated)")

# True — ничего не пишем на диск, только считаем и печатаем отчёт.
# Поставь False, когда прогонишь пару раз и убедишься, что отчёт адекватный.
DRY_RUN = False

# Делать ли бэкап оригинального ru.json перед перезаписью.
BACKUP_ORIGINALS = True

# False (по умолчанию) — моды без изменений и проблем просто считаются
# одной строкой в конце. True — вывести их тоже, но построчно (с числом
# ключей в словаре/в моде), а не одной длинной строкой через запятую.
SHOW_CLEAN_MODS = False

# Подстраховка: если случайно затешется файл конфига/манифеста -
# пропускаем его, а не патчим невпопад.
SKIP_SUFFIXES = ("-config.json", "-manifest.json")

# Регексп для подсчёта плейсхолдеров: {0}, {playerName}, %s, %d
PLACEHOLDER_RE = re.compile(r"\{[^{}]*\}|%[sd]")

# =========================================================================


def count_placeholders(text: str) -> int:
    return len(PLACEHOLDER_RE.findall(text))


def is_kv_pair_list(lst) -> bool:
    """Распознаёт формат '[["ключ", "значение"], ["ключ2", "значение2"], ...]',
    который некоторые моды используют вместо обычного JSON-объекта."""
    if not isinstance(lst, list) or not lst:
        return False
    keys = []
    for item in lst:
        if not (isinstance(item, list) and len(item) == 2 and isinstance(item[0], str)):
            return False
        keys.append(item[0])
    return len(keys) == len(set(keys))  # ключи должны быть уникальны, как в обычной карте


def flatten(obj, path=()):
    """Рекурсивно проходит dict/list/список-пар и отдаёт (путь_ключей, значение)
    для каждого листового значения (строка/число/bool/None)."""
    if isinstance(obj, dict):
        for k, v in obj.items():
            yield from flatten(v, path + (k,))
    elif isinstance(obj, list):
        if is_kv_pair_list(obj):
            for key, value in obj:
                yield from flatten(value, path + (key,))
        else:
            for i, v in enumerate(obj):
                yield from flatten(v, path + (i,))
    else:
        yield path, obj


def _kv_index_of(lst, key):
    for i, item in enumerate(lst):
        if item[0] == key:
            return i
    return None


def get_by_path(obj, path):
    node = obj
    for p in path:
        if isinstance(node, list) and is_kv_pair_list(node):
            node = node[_kv_index_of(node, p)][1]
        else:
            node = node[p]
    return node


def set_by_path(obj, path, value):
    node = obj
    for p in path[:-1]:
        if isinstance(node, list) and is_kv_pair_list(node):
            node = node[_kv_index_of(node, p)][1]
        else:
            node = node[p]
    last = path[-1]
    if isinstance(node, list) and is_kv_pair_list(node):
        node[_kv_index_of(node, last)][1] = value
    else:
        node[last] = value


def load_json(path: Path):
    raw = path.read_bytes()
    if raw.startswith(b"\xff\xfe") or raw.startswith(b"\xfe\xff"):
        text = raw.decode("utf-16")
    elif raw.startswith(b"\xef\xbb\xbf"):
        text = raw.decode("utf-8-sig")
    else:
        try:
            text = raw.decode("utf-8")
        except UnicodeDecodeError:
            # запасной вариант — иногда файлы сохранены в utf-16 без BOM
            text = raw.decode("utf-16")
    return json.loads(text)


def save_json(path: Path, data) -> None:
    with open(path, "w", encoding="utf-8", newline="\n") as f:
        json.dump(data, f, ensure_ascii=False, indent=4)
        f.write("\n")


def find_mod_dir(mod_name: str) -> Path | None:
    """Ищем папку мода с таким же именем, без учёта регистра."""
    if not MODS_DIR.is_dir():
        return None
    for entry in MODS_DIR.iterdir():
        if entry.is_dir() and entry.name.lower() == mod_name.lower():
            return entry
    return None


def find_ru_json_files(mod_dir: Path) -> list[Path]:
    return [p for p in mod_dir.rglob("ru.json")]


def find_localization_dirs_without_ru(mod_dir: Path) -> list[Path]:
    """Ищем папки, которые явно похожи на папки локализации (содержат
    en.json или другой *.json локали), но в которых нет ru.json —
    туда нужно скопировать наш словарь как fallback ru.json."""
    candidates = []
    for en_file in mod_dir.rglob("en.json"):
        folder = en_file.parent
        if not (folder / "ru.json").exists():
            candidates.append(folder)
    return candidates


def patch_mod(dict_path: Path, report: dict) -> None:
    """Обрабатывает один словарь. Ничего не печатает — всё складывает в report,
    рендерится потом одним куском в main()."""
    mod_name = dict_path.stem
    mod_dir = find_mod_dir(mod_name)

    if mod_dir is None:
        report["not_installed"].append(mod_name)
        return

    try:
        dictionary = load_json(dict_path)
    except Exception as e:
        report["read_errors"].append((mod_name, str(e)))
        return

    ru_files = find_ru_json_files(mod_dir)

    if not ru_files:
        empty_loc_dirs = find_localization_dirs_without_ru(mod_dir)
        if not empty_loc_dirs:
            report["no_localization_folder"].append(mod_name)
            return
        for folder in empty_loc_dirs:
            target = folder / "ru.json"
            report["ru_copied"].append((mod_name, str(target)))
            if not DRY_RUN:
                save_json(target, dictionary)
        return

    for ru_path in ru_files:
        try:
            target_data = load_json(ru_path)
        except Exception as e:
            report["read_errors"].append((f"{mod_name} ({ru_path.name})", str(e)))
            continue

        dict_flat = dict(flatten(dictionary))
        target_flat_paths = {p for p, _ in flatten(target_data)}

        matched = 0
        changed = 0
        placeholder_mismatch = 0
        dict_outdated = 0

        for path, dict_value in dict_flat.items():
            if path not in target_flat_paths:
                dict_outdated += 1
                continue
            original_value = get_by_path(target_data, path)
            if isinstance(dict_value, str) and isinstance(original_value, str):
                if count_placeholders(dict_value) != count_placeholders(original_value):
                    placeholder_mismatch += 1
                    continue
            if dict_value != original_value:
                changed += 1
            set_by_path(target_data, path, dict_value)
            matched += 1

        untranslated = len(target_flat_paths - set(dict_flat.keys()))

        entry = {
            "mod_name": mod_name,
            "rel_path": str(ru_path.relative_to(mod_dir)),
            "matched": matched,
            "changed": changed,
            "placeholder_mismatch": placeholder_mismatch,
            "dict_outdated": dict_outdated,
            "untranslated": untranslated,
            "dict_leaves": len(dict_flat),
            "target_leaves": len(target_flat_paths),
        }
        report["mod_results"].append(entry)

        report["totals"]["matched"] += matched
        report["totals"]["changed"] += changed
        report["totals"]["placeholder_mismatch"] += placeholder_mismatch
        report["totals"]["dict_outdated"] += dict_outdated
        report["totals"]["untranslated"] += untranslated

        if not DRY_RUN:
            if BACKUP_ORIGINALS:
                backup_path = ru_path.with_suffix(".original.json")
                shutil.copy2(ru_path, backup_path)
            save_json(ru_path, target_data)

        report["totals"]["mods_patched"] += 1


def is_notable(entry: dict) -> bool:
    return (
            entry["changed"] > 0
            or entry["placeholder_mismatch"] > 0
            or entry["dict_outdated"] > 0
            or entry["untranslated"] > 0
    )


def render_report(report: dict) -> None:
    W = 78
    print("=" * W)
    print("  COI RU LOCALIZATION PATCHER — " + ("DRY RUN (файлы не меняются)" if DRY_RUN else "РЕАЛЬНЫЙ ПРОГОН"))
    print("=" * W)

    if report["read_errors"]:
        print()
        print("ОШИБКИ ЧТЕНИЯ:")
        for name, err in report["read_errors"]:
            print(f"  ! {name}: {err}")

    if report["not_installed"]:
        print()
        print(f"МОД НЕ УСТАНОВЛЕН (словарь есть, папки мода нет) — {len(report['not_installed'])}:")
        for name in report["not_installed"]:
            print(f"  - {name}")

    if report["no_localization_folder"]:
        print()
        print(f"НЕТ ru.json И НЕТ ПАПКИ ЛОКАЛИЗАЦИИ (посмотри руками) — {len(report['no_localization_folder'])}:")
        for name in report["no_localization_folder"]:
            print(f"  - {name}")

    if report["ru_copied"]:
        print()
        print(f"СКОПИРОВАН СЛОВАРЬ КАК НОВЫЙ ru.json — {len(report['ru_copied'])}:")
        for name, target in report["ru_copied"]:
            print(f"  + {name} -> {target}")

    results = report["mod_results"]
    notable = [r for r in results if is_notable(r)]
    clean = [r for r in results if not is_notable(r)]

    if notable:
        print()
        name_w = max((len(r["mod_name"]) for r in notable), default=10)
        name_w = max(name_w, 10)

        # ширина = длина заголовка + запас, чтобы было куда центрировать
        col_w = {
            "dict": max(len("СЛОВАРЬ"), 3) + 4,
            "target": max(len("МОД"), 3) + 4,
            "changed": max(len("ЗАМЕНЕНО"), 3) + 4,
            "placeholder": max(len("ПЛЕЙСХ."), 3) + 4,
            "outdated": max(len("УСТАР."), 3) + 4,
            "untranslated": max(len("НЕПЕРЕВ."), 3) + 4,
        }

        header = (
            f"{'МОД':^{name_w}}  {'СЛОВАРЬ':^{col_w['dict']}}  {'МОД':^{col_w['target']}}  "
            f"{'ЗАМЕНЕНО':^{col_w['changed']}}  {'ПЛЕЙСХ.':^{col_w['placeholder']}}  "
            f"{'УСТАР.':^{col_w['outdated']}}  {'НЕПЕРЕВ.':^{col_w['untranslated']}}"
        )
        table_w = len(header)
        print("-" * table_w)
        print(header)
        print("-" * table_w)
        for r in sorted(notable, key=lambda x: (-x["changed"], x["mod_name"])):
            marker = "!" if r["placeholder_mismatch"] else ""
            print(
                f"{r['mod_name']:<{name_w}}  "
                f"{r['dict_leaves']:^{col_w['dict']}}  "
                f"{r['target_leaves']:^{col_w['target']}}  "
                f"{r['changed']:^{col_w['changed']}}  "
                f"{r['placeholder_mismatch']:^{col_w['placeholder']}}  "
                f"{r['dict_outdated']:^{col_w['outdated']}}  "
                f"{r['untranslated']:^{col_w['untranslated']}}"
                f"{('  ' + marker) if marker else ''}"
            )
        print("-" * table_w)

    if clean:
        if SHOW_CLEAN_MODS:
            print()
            print(f"БЕЗ ИЗМЕНЕНИЙ — {len(clean)} мод(ов):")
            name_w = max((len(r["mod_name"]) for r in clean), default=10)
            name_w = max(name_w, 10)
            for r in sorted(clean, key=lambda x: x["mod_name"]):
                print(f"  {r['mod_name']:<{name_w}}  словарь: {r['dict_leaves']:>4}   мод: {r['target_leaves']:>4}")
        else:
            print(f"Без изменений: {len(clean)} мод(ов) (SHOW_CLEAN_MODS = True, чтобы вывести построчно)")

    t = report["totals"]
    print()
    print("=" * W)
    print("ИТОГО:")
    print(f"  Обработано файлов ru.json:                {t['mods_patched']}")
    print(f"  Совпало ключей всего:                     {t['matched']}")
    print(f"  Из них реально изменено:                  {t['changed']}")
    print(f"  Пропущено из-за мисматча плейсхолдеров:   {t['placeholder_mismatch']}")
    print(f"  Устаревших ключей в словарях:             {t['dict_outdated']}")
    print(f"  Незалокализованных ключей у модов:        {t['untranslated']}")
    print("=" * W)

    if DRY_RUN:
        print()
        print("Это был тестовый прогон (DRY_RUN = True). Ничего не изменено.")
        print("Проверь отчёт выше и поставь DRY_RUN = False, чтобы применить изменения.")


def main():
    if not DICT_DIR.is_dir():
        print(f"Папка со словарями не найдена: {DICT_DIR}")
        return

    report = {
        "not_installed": [],
        "no_localization_folder": [],
        "ru_copied": [],
        "read_errors": [],
        "mod_results": [],
        "totals": {
            "mods_patched": 0,
            "matched": 0,
            "changed": 0,
            "placeholder_mismatch": 0,
            "dict_outdated": 0,
            "untranslated": 0,
        },
    }

    dict_files = sorted(DICT_DIR.glob("*.json"))
    skipped_special = [f for f in dict_files if f.name.endswith(SKIP_SUFFIXES)]
    dict_files = [f for f in dict_files if not f.name.endswith(SKIP_SUFFIXES)]

    for dict_path in dict_files:
        patch_mod(dict_path, report)

    render_report(report)


if __name__ == "__main__":
    main()