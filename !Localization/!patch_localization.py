#!/usr/bin/env python3
# -*- coding: utf-8 -*-

import datetime
import hashlib
import json
import os
import re
import shutil
import string
from pathlib import Path

# ============================== НАСТРОЙКИ ==============================

MODS_DIR = Path(r"C:\Users\www\AppData\Roaming\Captain of Industry\Mods")
DICT_DIR = Path(r"C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods (translated)")

# True — ничего не пишем на диск, только считаем и печатаем отчёт.
# Поставь False, когда прогонишь пару раз и убедишься, что отчёт адекватный.
DRY_RUN = False

# Делать ли бэкап оригинального ru.json перед перезаписью.
BACKUP_ORIGINALS = True

# Все служебные файлы скрипта (кэши, отчёты) хранятся тут, в одном месте.
LOGS_DIR = DICT_DIR.parent / "logs"

# Папка с "настройками" самого скрипта, которые ведёт пользователь руками
# (не путать с CONFIG_DICT_DIR — это словари для config.json модов).
SCRIPT_CONFIG_DIR = DICT_DIR.parent / "config"

# Единый файл-кэш "какая версия мода была на момент последнего бэкапа".
# Бэкап конкретного мода обновляется только если версия в его manifest.json
# отличается от записанной здесь.
BACKUP_VERSION_CACHE_PATH = LOGS_DIR / "backup_versions_cache.json"

# Где лежат твои словари для локализации самой игры и патч-нотов.
GAME_DICT_PATH = DICT_DIR.parent / "game" / "ru.json"
CHANGELOG_DICT_PATH = DICT_DIR.parent / "changelog" / "ru.json"

# Где лежат словари для манифестов (display_name/description_*) и конфигов
# (description у каждого параметра). Имя файла: "<ИмяПапкиМода>-manifest.json"
# и "<ИмяПапкиМода>-config.json" соответственно.
MANIFEST_DICT_DIR = DICT_DIR.parent / "mods (manifest)"
CONFIG_DICT_DIR = DICT_DIR.parent / "mods (config)"

# Папка для бэкапов оригинальных ru.json модов — отдельно от папок самих
# модов (при обновлении мода игровой менеджер стирает всю папку мода
# целиком, а вместе с ней и любой бэкап, что лежал внутри). Плоско, без
# подпапок: "<ИмяМода>-backup.json".
MOD_BACKUP_DIR = DICT_DIR.parent / "mods (backup)"

# Единственные поля manifest.json, которые вообще сравниваются/патчатся.
MANIFEST_FIELDS = ("display_name", "description", "description_short", "description_long")

# Сюда сохраняется однажды найденный путь к папке с игрой, чтобы не искать
# её заново при каждом запуске.
GAME_PATH_CACHE_PATH = SCRIPT_CONFIG_DIR / "game_path_cache.json"

# JSON-список (массив строк) с именами папок модов, которые не подлежат
# локализации в принципе (правят только внутренние инструменты игры, нет
# никаких строк для перевода). Такие моды исключаются из отчёта "НЕТ
# ЛОКАЛИЗАЦИИ ВООБЩЕ" — ведётся вручную.
NO_LOCALIZATION_WHITELIST_PATH = SCRIPT_CONFIG_DIR / "no_localization_whitelist.json"

# Сохранять ли подробный отчёт (с точными списками ключей, а не только
# счётчиками) в файл на диске при каждом прогоне — и dry-run, и реальном.
# Файл один, перезаписывается каждый раз (не копится куча файлов).
SAVE_REPORT_TO_FILE = True
REPORT_PATH = LOGS_DIR / "patch_report.json"

# Кэш "не изменился ли файл игры/патч-нотов с прошлого прогона" — по времени
# изменения файла (mtime). Если ни словарь, ни целевой файл не менялись —
# тяжёлое сравнение пропускается, а в отчёт подставляется сохранённый
# результат прошлого прогона. Только для игры/патч-нотов — на модах и так
# быстро. LOGIC_VERSION нужно вручную увеличивать при изменении логики
# сравнения, чтобы старый кэш не подставлял неактуальный результат.
GAME_SKIP_CACHE_PATH = LOGS_DIR / "game_skip_cache.json"
LOGIC_VERSION = 1

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
    if not isinstance(lst, list) or not lst:
        return False
    for item in lst:
        if not (isinstance(item, list) and len(item) >= 2 and isinstance(item[0], str)):
            return False
    return True


def find_kv_pair_list_violation(lst):
    if not isinstance(lst, list) or not lst:
        return None
    for i, item in enumerate(lst):
        if not isinstance(item, list):
            return i, item, "элемент — не список"
        if len(item) < 2:
            return i, item, f"длина {len(item)}, нужно минимум 2 (ключ + значение)"
        if not isinstance(item[0], str):
            return i, item, "первый элемент — не строка (ожидался ключ)"
    return None  # структура валидна (сюда не должны попадать при вызове после провала is_kv_pair_list)


def flatten(obj, path=()):
    if isinstance(obj, dict):
        for k, v in obj.items():
            yield from flatten(v, path + (k,))
    elif isinstance(obj, list):
        if is_kv_pair_list(obj):
            for item in obj:
                key, values = item[0], item[1:]
                for i, v in enumerate(values):
                    yield from flatten(v, path + (key, i))
        else:
            for i, v in enumerate(obj):
                yield from flatten(v, path + (i,))
    else:
        yield path, obj


def _kv_index_of(lst, key):
    found = None
    for i, item in enumerate(lst):
        if item[0] == key:
            found = i
    return found


def _kv_step(node, path, i):
    key, form_idx = path[i], path[i + 1]
    item = node[_kv_index_of(node, key)]
    return item[1 + form_idx], i + 2


def get_by_path(obj, path):
    node = obj
    i = 0
    while i < len(path):
        if isinstance(node, list) and is_kv_pair_list(node):
            node, i = _kv_step(node, path, i)
        else:
            node = node[path[i]]
            i += 1
    return node


def set_by_path(obj, path, value):
    node = obj
    i = 0
    while True:
        if isinstance(node, list) and is_kv_pair_list(node):
            key, form_idx = path[i], path[i + 1]
            item = node[_kv_index_of(node, key)]
            if i + 2 == len(path):
                item[1 + form_idx] = value
                return
            node = item[1 + form_idx]
            i += 2
        else:
            p = path[i]
            if i + 1 == len(path):
                node[p] = value
                return
            node = node[p]
            i += 1


def flatten_manifest_fields(data) -> dict:
    if not isinstance(data, dict):
        return {}
    return {(f,): data[f] for f in MANIFEST_FIELDS if f in data and isinstance(data[f], str)}


def manifest_container_exists(target_data, path) -> bool:
    # "контейнер" для полей манифеста — сам манифест целиком, он всегда есть
    return isinstance(target_data, dict)


def set_manifest_field(data: dict, path, value) -> None:
    data[path[0]] = value


CONFIG_EXCLUDED_FIELDS = {"default", "min", "max", "is_integer", "max_length"}


def flatten_config_fields(data) -> dict:
    if not isinstance(data, dict):
        return {}
    result = {}
    for param_id, param_obj in data.items():
        if not isinstance(param_obj, dict):
            continue
        for field, value in param_obj.items():
            if field in CONFIG_EXCLUDED_FIELDS:
                continue
            if isinstance(value, str):
                result[(param_id, field)] = value
    return result


def config_container_exists(target_data, path) -> bool:
    # "контейнер" для поля параметра — сам параметр (объект по его id),
    # который должен существовать в целевом файле, даже если конкретного
    # поля (например "name") в нём ещё нет
    param_id = path[0]
    return isinstance(target_data, dict) and isinstance(target_data.get(param_id), dict)


def set_config_field(data: dict, path, value) -> None:
    param_id, field = path
    data[param_id][field] = value


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


# Невидимые символы, которые лучше всегда хранить как \uXXXX-escape,
# а не сырым символом — иначе в редакторах они превращаются в мусорные
# плейсхолдеры (например мягкий перенос \u00AD выглядит как "SHY").
INVISIBLE_CHARS_TO_ESCAPE = {
    "\u00ad": "\\u00ad",  # soft hyphen / мягкий перенос
    "\u200b": "\\u200b",  # zero-width space
    "\u200c": "\\u200c",  # zero-width non-joiner
    "\u200d": "\\u200d",  # zero-width joiner
    "\u2060": "\\u2060",  # word joiner
    "\ufeff": "\\ufeff",  # BOM / zero-width no-break space
}


def escape_invisible_chars(text: str) -> str:
    for ch, esc in INVISIBLE_CHARS_TO_ESCAPE.items():
        text = text.replace(ch, esc)
    return text


def save_json(path: Path, data) -> None:
    text = json.dumps(data, ensure_ascii=False, indent=4)
    text = escape_invisible_chars(text)
    with open(path, "w", encoding="utf-8", newline="\n") as f:
        f.write(text)
        f.write("\n")


def compute_hash(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def _find_game_dir_via_steam_registry() -> Path | None:
    try:
        import winreg
    except ImportError:
        return None
    try:
        with winreg.OpenKey(winreg.HKEY_CURRENT_USER, r"Software\Valve\Steam") as key:
            steam_path = Path(winreg.QueryValueEx(key, "SteamPath")[0])
    except OSError:
        return None

    library_dirs = [steam_path]
    vdf_path = steam_path / "steamapps" / "libraryfolders.vdf"
    if vdf_path.is_file():
        try:
            text = vdf_path.read_text(encoding="utf-8", errors="ignore")
            for m in re.finditer(r'"path"\s*"([^"]+)"', text):
                p = Path(m.group(1).replace("\\\\", "\\"))
                if p not in library_dirs:
                    library_dirs.append(p)
        except OSError:
            pass

    for lib in library_dirs:
        candidate = lib / "steamapps" / "common" / "Captain of Industry"
        if (candidate / "Captain of Industry.exe").is_file():
            return candidate
    return None


def _find_game_dir_via_full_disk_scan() -> Path | None:
    skip_dirs = {"Windows", "$Recycle.Bin", "System Volume Information"}
    drives = [Path(f"{c}:\\") for c in string.ascii_uppercase if Path(f"{c}:\\").exists()]
    for drive in drives:
        for root, dirs, files in os.walk(drive, topdown=True, onerror=lambda e: None):
            dirs[:] = [d for d in dirs if d not in skip_dirs]
            if "Captain of Industry.exe" in files:
                return Path(root)
    return None


def find_game_directory() -> Path | None:
    found = _find_game_dir_via_steam_registry()
    if found is not None:
        return found
    print("Быстрый поиск через Steam не дал результата — сканирую все диски (может занять время)...")
    return _find_game_dir_via_full_disk_scan()


def resolve_game_directory() -> Path | None:
    if GAME_PATH_CACHE_PATH.is_file():
        try:
            cached = load_json(GAME_PATH_CACHE_PATH)
            game_dir = Path(cached.get("game_dir", ""))
            if game_dir.is_dir() and (game_dir / "Captain of Industry.exe").is_file():
                return game_dir
        except Exception:
            pass

    found = find_game_directory()
    if found is not None:
        GAME_PATH_CACHE_PATH.parent.mkdir(parents=True, exist_ok=True)
        save_json(GAME_PATH_CACHE_PATH, {"game_dir": str(found)})
    return found


def find_mod_dir(mod_name: str) -> Path | None:
    if not MODS_DIR.is_dir():
        return None
    for entry in MODS_DIR.iterdir():
        if entry.is_dir() and entry.name.lower() == mod_name.lower():
            return entry
    return None


def load_no_localization_whitelist(report: dict) -> set:
    """Моды из этого списка не подлежат локализации в принципе (правят
    только внутренние инструменты игры) — исключаются из отчёта "НЕТ
    ЛОКАЛИЗАЦИИ ВООБЩЕ". Ведётся вручную, JSON-массив строк."""
    if not NO_LOCALIZATION_WHITELIST_PATH.is_file():
        return set()
    try:
        names = load_json(NO_LOCALIZATION_WHITELIST_PATH)
    except Exception as e:
        report["read_errors"].append(("no_localization_whitelist.json", str(e)))
        return set()
    if not isinstance(names, list):
        report["read_errors"].append(("no_localization_whitelist.json", "ожидался JSON-массив строк"))
        return set()
    return {str(n).lower() for n in names}


def scan_installed_mods() -> list[Path]:
    if not MODS_DIR.is_dir():
        return []
    own_dir_name = DICT_DIR.parent.name  # "!Localization"
    result = []
    for entry in MODS_DIR.iterdir():
        if not entry.is_dir() or entry.name == own_dir_name:
            continue
        if (entry / "manifest.json").is_file():
            result.append(entry)
    return result


def _is_empty_json_file(path: Path) -> bool:
    try:
        return load_json(path) in ({}, [])
    except Exception:
        return False


def find_dict_file_for_mod(base_dir: Path, mod_folder_name: str, suffix: str) -> Path | None:
    if not base_dir.is_dir():
        return None
    target_name = f"{mod_folder_name}{suffix}".lower()
    for f in base_dir.glob("*.json"):
        if f.name.lower() == target_name:
            return f
    return None


def find_ru_json_files(mod_dir: Path) -> list[Path]:
    return [p for p in mod_dir.rglob("ru.json")]


def find_localization_dirs_without_ru(mod_dir: Path) -> list[Path]:
    candidates = []
    for en_file in mod_dir.rglob("en.json"):
        folder = en_file.parent
        if not (folder / "ru.json").exists():
            candidates.append(folder)
    return candidates


def read_mod_version(mod_dir: Path):
    manifest_path = mod_dir / "manifest.json"
    if not manifest_path.is_file():
        return None
    try:
        manifest = load_json(manifest_path)
        return manifest.get("version")
    except Exception:
        return None


def read_game_version(game_dir: Path):
    changelog_txt = game_dir / "changelog.txt"
    if not changelog_txt.is_file():
        return None
    try:
        with open(changelog_txt, "r", encoding="utf-8-sig", errors="ignore") as f:
            first_line = f.readline().strip()
        return first_line or None
    except Exception:
        return None


def load_backup_version_cache() -> dict:
    if not BACKUP_VERSION_CACHE_PATH.is_file():
        return {}
    try:
        return load_json(BACKUP_VERSION_CACHE_PATH)
    except Exception:
        return {}


def save_backup_version_cache(cache: dict) -> None:
    BACKUP_VERSION_CACHE_PATH.parent.mkdir(parents=True, exist_ok=True)
    save_json(BACKUP_VERSION_CACHE_PATH, dict(sorted(cache.items(), key=lambda kv: kv[0].lower())))


def load_game_skip_cache() -> dict:
    if not GAME_SKIP_CACHE_PATH.is_file():
        return {}
    try:
        return load_json(GAME_SKIP_CACHE_PATH)
    except Exception:
        return {}


def save_game_skip_cache(cache: dict) -> None:
    GAME_SKIP_CACHE_PATH.parent.mkdir(parents=True, exist_ok=True)
    save_json(GAME_SKIP_CACHE_PATH, cache)


def patch_one_file(
        name: str,
        dictionary,
        target_path: Path,
        version_marker,
        backup_cache: dict,
        report: dict,
        category: str = "mod",
        flatten_fn=None,
        target_flatten_fn=None,
        set_fn=None,
        do_backup: bool = True,
        skip_if_empty: bool = False,
        container_exists_fn=None,
        backup_path: Path = None,
) -> bool:
    try:
        target_data = load_json(target_path)
    except Exception as e:
        report["read_errors"].append((f"{name} ({target_path.name})", str(e)))
        return False

    if skip_if_empty and (dictionary in ({}, []) or target_data in ({}, [])):
        return False

    use_default_flatten = flatten_fn is None
    flatten_fn = flatten_fn or (lambda obj: dict(flatten(obj)))
    target_flatten_fn = target_flatten_fn or flatten_fn
    set_fn = set_fn or set_by_path

    if use_default_flatten:
        for label, obj, path_hint in (("словарь", dictionary, "dict"), ("файл мода/игры", target_data, target_path.name)):
            if isinstance(obj, list) and not is_kv_pair_list(obj):
                violation = find_kv_pair_list_violation(obj)
                if violation is not None:
                    idx, item, reason = violation
                    report["read_errors"].append((
                        name,
                        f"{label} ({path_hint}) похож на список пар, но элемент №{idx} не подходит "
                        f"({reason}): {item!r}"
                    ))
                    return False

    dict_flat = flatten_fn(dictionary)
    target_flat = target_flatten_fn(target_data)

    if container_exists_fn is not None:
        # Поле есть у нас в словаре, но отсутствует в целевом файле — если
        # его "контейнер" (сам параметр/сам манифест) при этом существует,
        # считаем это полем на добавление, а не устаревшей записью.
        for path in dict_flat:
            if path not in target_flat and container_exists_fn(target_data, path):
                target_flat[path] = ""

    matched = 0
    changed = 0
    placeholder_mismatch = 0
    dict_outdated = 0

    outdated_keys = []
    placeholder_mismatch_details = []
    changed_details = []

    for path, dict_value in dict_flat.items():
        if path not in target_flat:
            dict_outdated += 1
            outdated_keys.append((path, dict_value))
            continue
        original_value = target_flat[path]
        if isinstance(dict_value, str) and isinstance(original_value, str):
            if count_placeholders(dict_value) != count_placeholders(original_value):
                placeholder_mismatch += 1
                placeholder_mismatch_details.append((path, original_value, dict_value))
        if dict_value != original_value:
            changed += 1
            changed_details.append((path, original_value, dict_value))
        set_fn(target_data, path, dict_value)
        matched += 1

    untranslated_keys = sorted(
        (p for p in target_flat if p not in dict_flat and target_flat[p] != ""),
        key=lambda p: [str(x) for x in p],
    )
    untranslated = len(untranslated_keys)

    entry = {
        "mod_name": name,
        "category": category,
        "rel_path": target_path.name,
        "matched": matched,
        "changed": changed,
        "placeholder_mismatch": placeholder_mismatch,
        "dict_outdated": dict_outdated,
        "untranslated": untranslated,
        "dict_leaves": len(dict_flat),
        "target_leaves": len(target_flat),
        "outdated_keys": outdated_keys,
        "placeholder_mismatch_details": placeholder_mismatch_details,
        "changed_details": changed_details,
        "untranslated_keys": [(p, target_flat[p]) for p in untranslated_keys],
    }
    report["mod_results"].append(entry)

    report["totals"]["matched"] += matched
    report["totals"]["changed"] += changed
    report["totals"]["placeholder_mismatch"] += placeholder_mismatch
    report["totals"]["dict_outdated"] += dict_outdated
    report["totals"]["untranslated"] += untranslated

    should_backup = False
    if do_backup:
        cached_marker = backup_cache.get(name)

        if not backup_path.exists():
            should_backup = True
        elif version_marker is not None:
            should_backup = cached_marker != version_marker
        else:
            should_backup = False

        if should_backup and backup_path.exists() and version_marker is not None:
            report["backup_refreshed"].append((name, cached_marker, version_marker))

        if not DRY_RUN and BACKUP_ORIGINALS and should_backup:
            backup_path.parent.mkdir(parents=True, exist_ok=True)
            shutil.copy2(target_path, backup_path)

    if not DRY_RUN:
        save_json(target_path, target_data)

    report["totals"]["mods_patched"] += 1
    return should_backup


def patch_mod(dict_path: Path, report: dict, backup_cache: dict) -> None:
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
    mod_version = read_mod_version(mod_dir)

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
        backup_path = MOD_BACKUP_DIR / f"{mod_name}-backup.json"
        did_backup = patch_one_file(mod_name, dictionary, ru_path, mod_version, backup_cache, report, backup_path=backup_path)
        if not DRY_RUN and did_backup and mod_version is not None:
            backup_cache[mod_name] = mod_version


def patch_manifest(mod_dir: Path, mod_folder_name: str, dict_path: Path, report: dict, backup_cache: dict) -> None:
    manifest_path = mod_dir / "manifest.json"
    try:
        dictionary = load_json(dict_path)
    except Exception as e:
        report["read_errors"].append((f"{mod_folder_name}-manifest", str(e)))
        return
    patch_one_file(
        f"{mod_folder_name}-manifest", dictionary, manifest_path, None, backup_cache, report,
        category="manifest", flatten_fn=flatten_manifest_fields, set_fn=set_manifest_field,
        do_backup=False, container_exists_fn=manifest_container_exists,
    )


def patch_config(mod_dir: Path, mod_folder_name: str, dict_path: Path, report: dict, backup_cache: dict) -> None:
    config_path = mod_dir / "config.json"
    try:
        dictionary = load_json(dict_path)
    except Exception as e:
        report["read_errors"].append((f"{mod_folder_name}-config", str(e)))
        return
    patch_one_file(
        f"{mod_folder_name}-config", dictionary, config_path, None, backup_cache, report,
        category="config", flatten_fn=flatten_config_fields, set_fn=set_config_field,
        do_backup=False, skip_if_empty=True, container_exists_fn=config_container_exists,
    )


def patch_manifests_and_configs(report: dict, backup_cache: dict) -> None:
    no_loc_whitelist = load_no_localization_whitelist(report)

    for mod_dir in scan_installed_mods():
        mod_name = mod_dir.name

        has_localization = bool(find_ru_json_files(mod_dir)) or bool(find_localization_dirs_without_ru(mod_dir))
        if not has_localization:
            if mod_name.lower() not in no_loc_whitelist:
                report["no_localization_mods"].append(mod_name)
        elif find_dict_file_for_mod(DICT_DIR, mod_name, ".json") is None:
            report["translation_dict_missing"].append(mod_name)

        manifest_dict = find_dict_file_for_mod(MANIFEST_DICT_DIR, mod_name, "-manifest.json")
        if manifest_dict is None:
            report["manifest_dict_missing"].append(mod_name)
        else:
            patch_manifest(mod_dir, mod_name, manifest_dict, report, backup_cache)

        config_target = mod_dir / "config.json"
        if config_target.is_file() and not _is_empty_json_file(config_target):
            config_dict = find_dict_file_for_mod(CONFIG_DICT_DIR, mod_name, "-config.json")
            if config_dict is None:
                report["config_dict_missing"].append(mod_name)
            else:
                patch_config(mod_dir, mod_name, config_dict, report, backup_cache)


def is_notable(entry: dict) -> bool:
    return (
            entry["changed"] > 0
            or entry["placeholder_mismatch"] > 0
            or entry["dict_outdated"] > 0
            or entry["untranslated"] > 0
    )


def format_path(path) -> str:
    return " / ".join(str(p) for p in path)


RU_MONTHS_GENITIVE = [
    "Января", "Февраля", "Марта", "Апреля", "Мая", "Июня",
    "Июля", "Августа", "Сентября", "Октября", "Ноября", "Декабря",
]


def format_ru_datetime(dt: datetime.datetime) -> str:
    return f"{dt.day} {RU_MONTHS_GENITIVE[dt.month - 1]} {dt.year} / {dt.strftime('%H:%M:%S')}"


def _esc(value) -> str:
    return json.dumps(value, ensure_ascii=False)


def _write_entry_lines(fields: list, indent: int) -> list:
    pad = " " * indent
    inner = " " * (indent + 4)
    lines = [f"{pad}{{"]
    for i, (k, v) in enumerate(fields):
        comma = "," if i < len(fields) - 1 else ""
        lines.append(f"{inner}{_esc(k)}: {_esc(v)}{comma}")
    lines.append(f"{pad}}}")
    return lines


def _write_entry_array_lines(entries_fields: list, indent: int) -> list:
    pad = " " * indent
    if not entries_fields:
        return [f"{pad}[]"]
    lines = [f"{pad}["]
    for i, fields in enumerate(entries_fields):
        entry_lines = _write_entry_lines(fields, indent + 4)
        if i < len(entries_fields) - 1:
            entry_lines[-1] += ","
        lines.extend(entry_lines)
    lines.append(f"{pad}]")
    return lines


def _write_mod_block_lines(name: str, file: str, sections: list, indent: int) -> list:
    pad = " " * indent
    inner = " " * (indent + 4)
    lines = [f"{pad}{{"]
    lines.append(f"{inner}{_esc('name')}: {_esc(name)},")
    lines.append(f"{inner}{_esc('file')}: {_esc(file)}{',' if sections else ''}")
    for si, (sec_name, entries_fields) in enumerate(sections):
        lines.append("")  # пустая строка перед каждой секцией — визуальный блок
        sec_lines = _write_entry_array_lines(entries_fields, indent + 4)
        sec_lines[0] = f"{inner}{_esc(sec_name)}: " + sec_lines[0].lstrip()
        if si < len(sections) - 1:
            sec_lines[-1] += ","
        lines.extend(sec_lines)
    lines.append(f"{pad}}}")
    return lines


def _write_mod_block_array_lines(mod_blocks_lines: list, indent: int) -> list:
    pad = " " * indent
    if not mod_blocks_lines:
        return [f"{pad}[]"]
    lines = [f"{pad}["]
    for i, block_lines in enumerate(mod_blocks_lines):
        if i > 0:
            lines.append("")  # пустая строка между модами/записями
        block = list(block_lines)
        if i < len(mod_blocks_lines) - 1:
            block[-1] += ","
        lines.extend(block)
    lines.append(f"{pad}]")
    return lines


def _build_mod_entry_blocks(entries: list, indent: int) -> list:
    blocks = []
    for entry in sorted(entries, key=lambda r: r["mod_name"].lower()):
        has_detail = entry["outdated_keys"] or entry["untranslated_keys"] or entry["placeholder_mismatch_details"]
        if not has_detail:
            continue  # всё совпало — этому моду в отчёте делать нечего

        sections = []
        if entry["outdated_keys"]:
            sections.append((
                "устарело в словаре",
                [[("ключ", format_path(path)), ("значение", value)] for path, value in entry["outdated_keys"]],
            ))
        if entry["untranslated_keys"]:
            sections.append((
                "отсутствующий перевод",
                [[("ключ", format_path(path)), ("текущее_значение", value)] for path, value in entry["untranslated_keys"]],
            ))
        if entry["placeholder_mismatch_details"]:
            sections.append((
                "несовпадающие плейсхолдеры",
                [
                    [("ключ", format_path(path)), ("оригинальное значение", orig), ("значение в словаре", dict_val)]
                    for path, orig, dict_val in entry["placeholder_mismatch_details"]
                ],
            ))

        blocks.append(_write_mod_block_lines(entry["mod_name"], entry["rel_path"], sections, indent))
    return blocks


def _write_string_array_lines(strings: list, indent: int) -> list:
    pad = " " * indent
    if not strings:
        return [f"{pad}[]"]
    inner = " " * (indent + 4)
    lines = [f"{pad}["]
    for i, s in enumerate(strings):
        comma = "," if i < len(strings) - 1 else ""
        lines.append(f"{inner}{_esc(s)}{comma}")
    lines.append(f"{pad}]")
    return lines


def _append_named_section(lines: list, key: str, section_lines: list, trailing_comma: bool) -> None:
    section_lines = list(section_lines)
    section_lines[0] = f'    {_esc(key)}: ' + section_lines[0].lstrip()
    if trailing_comma:
        section_lines[-1] += ","
    lines.extend(section_lines)


def save_detailed_report(report: dict) -> Path:
    by_category = {cat: [] for cat in ("mod", "game", "manifest", "config")}
    for e in report["mod_results"]:
        by_category.setdefault(e.get("category", "mod"), []).append(e)

    mods_blocks = _build_mod_entry_blocks(by_category["mod"], indent=8)
    game_blocks = _build_mod_entry_blocks(by_category["game"], indent=8)
    manifest_blocks = _build_mod_entry_blocks(by_category["manifest"], indent=8)
    config_blocks = _build_mod_entry_blocks(by_category["config"], indent=8)

    lines = ["{"]
    lines.append(f'    {_esc("дата")}: {_esc(format_ru_datetime(datetime.datetime.now()))},')
    lines.append(f'    {_esc("режим")}: {_esc("dry-run" if DRY_RUN else "real-run")},')
    lines.append("")

    _append_named_section(lines, "моды без локализации",
                          _write_string_array_lines(report["no_localization_mods"], indent=4), True)
    lines.append("")

    _append_named_section(lines, "переводы без словаря",
                          _write_string_array_lines(report["translation_dict_missing"], indent=4), True)
    lines.append("")

    _append_named_section(lines, "манифесты без словаря",
                          _write_string_array_lines(report["manifest_dict_missing"], indent=4), True)
    lines.append("")
    _append_named_section(lines, "конфиги без словаря",
                          _write_string_array_lines(report["config_dict_missing"], indent=4), True)
    lines.append("")

    _append_named_section(lines, "моды", _write_mod_block_array_lines(mods_blocks, indent=4), True)
    lines.append("")
    _append_named_section(lines, "основная игра", _write_mod_block_array_lines(game_blocks, indent=4), True)
    lines.append("")
    _append_named_section(lines, "манифесты", _write_mod_block_array_lines(manifest_blocks, indent=4), True)
    lines.append("")
    _append_named_section(lines, "конфиги", _write_mod_block_array_lines(config_blocks, indent=4), False)

    lines.append("}")

    REPORT_PATH.parent.mkdir(parents=True, exist_ok=True)
    with open(REPORT_PATH, "w", encoding="utf-8", newline="\n") as f:
        f.write("\n".join(lines))
        f.write("\n")
    return REPORT_PATH


def render_report(report: dict) -> None:
    W = 78
    print("=" * W)
    print("  COI RU LOCALIZATION PATCHER — " + ("DRY RUN (файлы не меняются)" if DRY_RUN else "РЕАЛЬНЫЙ ПРОГОН"))
    print("=" * W)

    if report["game_not_found"]:
        print()
        print("! Папка с игрой не найдена — локализация игры и патч-ноты не тронуты.")
        print("  Проверь, что игра установлена, или пропиши путь вручную в game_path_cache.json.")
    elif report["game_dir"]:
        print()
        print(f"Игра найдена: {report['game_dir']}")

    if report["game_skip_used"]:
        print(f"(пропущено по кэшу, файлы не менялись: {', '.join(report['game_skip_used'])})")

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

    if report["backup_refreshed"]:
        print()
        verb = "будет обновлён" if DRY_RUN else "обновлён"
        print(f"БЭКАП {verb.upper()} (версия/содержимое изменилось) — {len(report['backup_refreshed'])}:")
        for name, old_v, new_v in report["backup_refreshed"]:
            def short(v):
                if v is None:
                    return "неизвестно"
                return v if len(v) <= 16 else v[:8] + "…"
            print(f"  - {name}: {short(old_v)} -> {short(new_v)}")

    if report["no_localization_mods"]:
        print()
        print(f"НЕТ ЛОКАЛИЗАЦИИ ВООБЩЕ (в моде нет ни ru.json, ни en.json — переводить пока нечего) — {len(report['no_localization_mods'])}:")
        for name in report["no_localization_mods"]:
            print(f"  - {name}")

    if report["translation_dict_missing"]:
        print()
        print(f"НЕТ СЛОВАРЯ ПЕРЕВОДА (в моде есть локализация, но файла в «mods (translated)» нет) — {len(report['translation_dict_missing'])}:")
        for name in report["translation_dict_missing"]:
            print(f"  - {name}")

    if report["manifest_dict_missing"]:
        print()
        print(f"НЕТ СЛОВАРЯ МАНИФЕСТА (мод установлен, файла в «mods (manifests)» нет) — {len(report['manifest_dict_missing'])}:")
        for name in report["manifest_dict_missing"]:
            print(f"  - {name}")

    if report["config_dict_missing"]:
        print()
        print(f"НЕТ СЛОВАРЯ КОНФИГА (в моде есть config.json, но файла в «mods (configs)» нет) — {len(report['config_dict_missing'])}:")
        for name in report["config_dict_missing"]:
            print(f"  - {name}")

    render_category_table("МОДЫ", [r for r in report["mod_results"] if r.get("category", "mod") == "mod"])
    render_category_table("ОСНОВНАЯ ИГРА", [r for r in report["mod_results"] if r.get("category") == "game"])
    render_category_table("МАНИФЕСТЫ", [r for r in report["mod_results"] if r.get("category") == "manifest"])
    render_category_table("КОНФИГИ", [r for r in report["mod_results"] if r.get("category") == "config"])

    t = report["totals"]
    print()
    print("=" * W)
    print("ИТОГО:")
    print(f"  Обработано файлов ru.json:                {t['mods_patched']}")
    print(f"  Совпало ключей всего:                     {t['matched']}")
    print(f"  Из них реально изменено:                  {t['changed']}")
    print(f"  Плейсхолдер-мисматч (стоит проверить):    {t['placeholder_mismatch']}")
    print(f"  Устаревших ключей в словарях:             {t['dict_outdated']}")
    print(f"  Незалокализованных ключей в модах:        {t['untranslated']}")
    print("=" * W)

    if DRY_RUN:
        print()
        print("Это был тестовый прогон (DRY_RUN = True). Ничего не изменено.")
        print("Проверь отчёт выше и поставь DRY_RUN = False, чтобы применить изменения.")


def render_category_table(title: str, entries: list) -> None:
    if not entries:
        return

    notable = [r for r in entries if is_notable(r)]
    clean = [r for r in entries if not is_notable(r)]

    print()
    print(f"### {title}")

    if notable:
        name_w = max((len(r["mod_name"]) for r in notable), default=10)
        name_w = max(name_w, 10)

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
        for r in sorted(notable, key=lambda x: x["mod_name"].lower()):
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
            name_w = max((len(r["mod_name"]) for r in clean), default=10)
            name_w = max(name_w, 10)
            for r in sorted(clean, key=lambda x: x["mod_name"]):
                print(f"  {r['mod_name']:<{name_w}}  словарь: {r['dict_leaves']:>4}   мод: {r['target_leaves']:>4}")
        else:
            print(f"Без изменений: {len(clean)} (SHOW_CLEAN_MODS = True, чтобы вывести построчно)")


def apply_cached_entry(entry: dict, report: dict) -> None:
    """Подставляет в report сохранённый с прошлого прогона результат — без
    повторного сравнения. Зеркалит финальные шаги patch_one_file."""
    report["mod_results"].append(entry)
    report["totals"]["matched"] += entry["matched"]
    report["totals"]["changed"] += entry["changed"]
    report["totals"]["placeholder_mismatch"] += entry["placeholder_mismatch"]
    report["totals"]["dict_outdated"] += entry["dict_outdated"]
    report["totals"]["untranslated"] += entry["untranslated"]
    report["totals"]["mods_patched"] += 1


def patch_game_and_changelog(report: dict, backup_cache: dict, skip_cache: dict) -> None:
    game_dir = resolve_game_directory()
    if game_dir is None:
        report["game_not_found"] = True
        return

    report["game_dir"] = str(game_dir)
    translations_dir = game_dir / "Translations"
    game_version = read_game_version(game_dir)

    targets = [
        ("Игра (Translations/ru.json)", GAME_DICT_PATH, translations_dir / "ru.json",
         DICT_DIR.parent / "game" / "ru-backup.json"),
        ("Патч-ноты (Changelog/ru.json)", CHANGELOG_DICT_PATH, translations_dir / "Changelog" / "ru.json",
         DICT_DIR.parent / "changelog" / "ru-backup.json"),
    ]

    for name, dict_path, target_path, backup_path in targets:
        if not dict_path.is_file():
            continue  # своего словаря для этой цели пока нет — нечего патчить
        if not target_path.is_file():
            report["read_errors"].append((name, f"файл не найден: {target_path}"))
            continue

        dict_mtime = dict_path.stat().st_mtime
        target_mtime = target_path.stat().st_mtime
        cached = skip_cache.get(name)

        if (
                cached is not None
                and cached.get("logic_version") == LOGIC_VERSION
                and cached.get("dict_mtime") == dict_mtime
                and cached.get("target_mtime") == target_mtime
                and (DRY_RUN or cached.get("was_real_write"))
        ):
            # ни словарь, ни файл игры не менялись с прошлого прогона —
            # сравнение точно даст тот же результат, пересчитывать незачем.
            # На реальном прогоне доверяем только кэшу от ТАКОГО ЖЕ реального
            # прогона — иначе можно пропустить запись, которая ещё ни разу
            # не была сделана (кэш от dry-run ничего на диск не писал).
            apply_cached_entry(cached["entry"], report)
            report["game_skip_used"].append(name)
            continue

        try:
            dictionary = load_json(dict_path)
        except Exception as e:
            report["read_errors"].append((name, str(e)))
            continue

        version_marker = game_version if game_version is not None else compute_hash(target_path)
        did_backup = patch_one_file(name, dictionary, target_path, version_marker, backup_cache, report, category="game", backup_path=backup_path)
        if not DRY_RUN and did_backup and version_marker is not None:
            backup_cache[name] = version_marker

        # mtime цели могли сдвинуть записью выше — берём актуальный, чтобы
        # следующий прогон сравнивал именно с тем, что реально на диске
        new_target_mtime = target_path.stat().st_mtime
        skip_cache[name] = {
            "logic_version": LOGIC_VERSION,
            "dict_mtime": dict_mtime,
            "target_mtime": new_target_mtime,
            "was_real_write": not DRY_RUN,
            "entry": report["mod_results"][-1],
        }


def main():
    if not DICT_DIR.is_dir():
        print(f"Папка со словарями не найдена: {DICT_DIR}")
        return

    report = {
        "not_installed": [],
        "no_localization_folder": [],
        "ru_copied": [],
        "read_errors": [],
        "backup_refreshed": [],
        "mod_results": [],
        "game_not_found": False,
        "game_dir": None,
        "manifest_dict_missing": [],
        "no_localization_mods": [],
        "translation_dict_missing": [],
        "config_dict_missing": [],
        "game_skip_used": [],
        "totals": {
            "mods_patched": 0,
            "matched": 0,
            "changed": 0,
            "placeholder_mismatch": 0,
            "dict_outdated": 0,
            "untranslated": 0,
        },
    }

    backup_cache = load_backup_version_cache()
    skip_cache = load_game_skip_cache()

    dict_files = sorted(DICT_DIR.glob("*.json"))
    skipped_special = [f for f in dict_files if f.name.endswith(SKIP_SUFFIXES)]
    dict_files = [f for f in dict_files if not f.name.endswith(SKIP_SUFFIXES)]

    for dict_path in dict_files:
        patch_mod(dict_path, report, backup_cache)

    patch_game_and_changelog(report, backup_cache, skip_cache)
    patch_manifests_and_configs(report, backup_cache)

    if not DRY_RUN:
        save_backup_version_cache(backup_cache)
    save_game_skip_cache(skip_cache)

    render_report(report)

    if SAVE_REPORT_TO_FILE:
        report_path = save_detailed_report(report)
        print()
        print(f"Подробный отчёт сохранён: {report_path}")


if __name__ == "__main__":
    main()
