# 🛡️ Paradox Security Suite

> Advanced Unity build protection, obfuscation, integrity validation, and anti-reverse-engineering toolkit for shipping builds.

![Unity](https://img.shields.io/badge/Unity-2021%2B-black?logo=unity)
![Version](https://img.shields.io/badge/Version-1.0.0-blue)
![Platform](https://img.shields.io/badge/Platform-Windows-lightgrey)
![License](https://img.shields.io/badge/License-GPLv3-blue)

---

# 🔥 Overview

**Paradox Security Suite** is a powerful security and obfuscation framework designed for Unity developers who want to make reverse engineering, asset ripping, tampering, and runtime injection significantly harder in shipping builds.

Built for:

* Unity 6.2
* Unity 6.0 LTS
* Unity 2022 LTS+
* Unity 2021 LTS+

The suite provides:

* File obfuscation
* Scene GameObject obfuscation
* Asset renaming
* AES encryption support
* SHA-256 integrity validation
* Runtime anti-cheat systems
* Configurable whitelist system
* Build protection utilities

---

# ✨ Features

## 🔒 File Obfuscation

Rename and protect supported Unity asset files automatically.

Supported file types include:

* `.cs`
* `.prefab`
* `.unity`
* `.shader`
* `.mat`
* `.asset`
* `.anim`
* and more

Helps reduce readability and increases reverse engineering difficulty.

---

## 🎮 Scene GameObject Obfuscation

Automatically renames GameObjects inside open Unity scenes to make scene hierarchy analysis harder.

Example:

Before:
PlayerCharacter
EnemySpawner
LootChest

After:
xA12_f9
pQ77_kL
zz0_b3

---

## 🧠 Asset Main Object Name Fixing

Synchronizes Unity asset internal object names with renamed file names to avoid broken references and mismatches.

---

## 📜 Obfuscation Logging

Every rename and protection operation is logged to:

`obfuscation_log.txt`

Useful for:

* Debugging
* Tracking renamed assets
* Restoring mappings
* Verification

---

# 🔐 Integrity & Hashing System

## Generate SHA-256 Hashes

Creates secure hashes for supported project files.

Used for:

* Tamper detection
* Build verification
* File integrity validation

---

## Validate SHA-256 Hashes

Compares current files against saved hashes to detect:

* Modified files
* Unauthorized edits
* Tampering attempts

---

# 🛡️ Runtime Anti-Cheat

Integrated runtime protection system designed to make memory injection and debugger usage more difficult.

Features include:

* Basic debugger detection
* Injector resistance
* Runtime validation checks
* Tamper monitoring

Anti-cheat can be enabled or disabled directly from the suite UI.

---

# ⚙️ Configurable Settings

Paradox Security Suite includes multiple configurable protection settings.

## Available Options

* Preserve String Literals
* Dry Run Mode
* Enable Logging
* AES Encryption
* Encrypt File Types
* Obfuscation Seed
* XOR Key
* AES Key & IV

---

# 📂 Whitelist System

Exclude files or folders from obfuscation using:

`whitelist.json`

Recommended for:

* Third-party plugins
* Steamworks
* SDKs
* External libraries
* Middleware

Example:

```json
[
  "Assets/Plugins/Steamworks",
  "Assets/TextMesh Pro",
  "Assets/ThirdParty"
]
```

---

# 🚀 Installation

## Step 1 — Backup Your Project

Before running any obfuscation:

Use:

`Export Backup (.unitypackage)`

This creates a full backup of your project assets.

---

## Step 2 — Import Package

Import the Paradox Security Suite package into your Unity project.

---

## Step 3 — Configure Whitelist

Edit:

`whitelist.json`

Add folders/files you want excluded from protection.

---

## Step 4 — Configure Protection Settings

Choose your:

* Encryption settings
* Logging settings
* Obfuscation settings
* Runtime protection settings

---

## Step 5 — Run Obfuscation

Use the provided tools to:

* Obfuscate files
* Rename GameObjects
* Generate hashes
* Encrypt supported assets

---

# 📖 Workflow Guide

## 1. Backup

Always create a backup before obfuscating.

---

## 2. Configure Whitelist

Exclude plugins and sensitive dependencies.

---

## 3. Obfuscate Assets

Protect files and scene objects.

---

## 4. Generate Hashes

Create SHA-256 integrity records.

---

## 5. Build Your Game

Create your shipping build.

---

## 6. Enable Anti-Cheat

Activate runtime protection features.

---

# ❓ FAQ

## Why should I backup before obfuscation?

Obfuscation renames files and scene objects. A backup allows you to safely restore the original project if needed.

---

## What files are supported?

Common Unity asset types including:

* `.cs`
* `.prefab`
* `.shader`
* `.mat`
* `.unity`
* and more

---

## How do I whitelist files?

Edit:

`whitelist.json`

and add the file/folder paths you want excluded.

---

## Should third-party assets be whitelisted?

Yes.

External plugins and SDKs should usually be excluded to avoid breaking references.

Examples:

* Steamworks
* Photon
* FMOD
* Odin Inspector
* TextMesh Pro

---

## Can this stop all reverse engineering?

No protection is completely unbreakable.

Paradox Security Suite is designed to:

* Increase reverse engineering difficulty
* Slow down attackers
* Protect assets
* Detect tampering
* Harden shipping builds

---

# ⚠️ Important Notes

* Always test builds after obfuscation
* Always keep backups
* Avoid obfuscating critical third-party SDKs unless tested
* Some protections may increase build processing time

---

# 🧩 Planned Features

* IL2CPP hardening tools
* Advanced metadata stripping
* Runtime memory encryption
* Assembly virtualization
* Online license validation
* Build fingerprinting
* Cloud integrity verification

---

# 📜 License

Licensed under the GNU General Public License v3.0 (GPL-3.0).

You are free to use, modify, and distribute this software under the terms of the GPL-3.0 license.

See the LICENSE file for full details.

---

# 👨‍💻 Author

Created by KearinL

Unity Developer • Security Systems • Game Technology

---

# ⭐ Support

If you like this project:

* Star the repository
* Share feedback
* Report issues
* Contribute ideas

---
