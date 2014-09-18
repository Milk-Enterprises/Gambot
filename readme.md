# Gambot
## The Revenge
Basically XKCD's [Bucket](https://github.com/zigdon/xkcd-Bucket/) rewritten in C# with modularity out the wazoo.

### Design Goals
1. Clean code -- should be readable and maintainable
2. Modularity -- the core should be as small as possible, with all features implemented as plugins
3. Compatibility -- should be compatible with any Bucket database

### Module Design
Modules:
1. Can define "magic variables" (such as $who, $giveitem, $someone) through `Variables` class
2. Can read/write its own configuration data through `Config` class
3. Can read/write its own SQL tables
4. Can list other modules as dependencies and communicate with them (how??) 

### Problem Areas
1. Conjucation -- $verbed, $verbing, $nouns, etc.
2. Genders -- Keeping track of the last person variable
    * Probably doable if the $who/$to/$someone module can keep an internal reference to the last $who/$to/$someone reference and use that for further $he/$him/$his references
3. Casing - $who/$Who/$WHO

### Other Notes
1. Variables are a part of the Gambot core, since almost all modules will make use of them
2. While Bucket uses MySQL, Gambot will use SQLite. Ideally, the schemas will be compatible
    * If we decide to break backwards compatibility, a migration script should be made