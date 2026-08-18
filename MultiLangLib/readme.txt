MultiLangLib 0.1.0

Install the complete MultiLangLib directory in:
%APPDATA%\Captain of Industry\Mods\MultiLangLib

Consumer mods must declare "MultiLangLib>=0.1.0" in mod_dependencies and
compile against MultiLangLib.dll without copying that DLL into their own mod.

Usage:
  string title = Lang.Get("multilanglib.MyMod.menu.title");
  LocStrFormatted label = Lang.Localized("multilanglib.MyMod.menu.title");

Set debug_language=true in the MultiLangLib mod settings to display keys.
See the included README.md for file formats, search order, and examples.
