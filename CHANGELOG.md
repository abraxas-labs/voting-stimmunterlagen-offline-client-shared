# ✨ Changelog (`v2.2.1`)

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## Version Info

```text
This version -------- v2.2.1
Previous version ---- v2.2.0
Initial version ----- v1.3.4
Total commits ------- 1
```

## [v2.2.1] - 2025-03-03

### ❌ Removed

- remove attachment stations from municipality

## [v2.2.0] - 2024-10-18

### 🆕 Added

- add stistat flag to municipality

## [v2.1.4] - 2024-08-28

### :arrows_counterclockwise: Changed

- update bug bounty template reference
- patch ci-cd template version, align with new defaults

## [v2.1.3] - 2024-08-13

### 🔄 Changed

- enable continuous integration build property for dotnet CLI
- preserve source revision information version to enable source-link feature

## [v2.1.2] - 2024-08-12

### 🔄 Changed

- use ci build property to enable normalized artifacts for official builds.

## [v2.1.1] - 2024-08-12

### 🔄 Changed

- prevent source revision from being included in release builds to preserve deterministic builds.

## [v2.1.0] - 2024-07-04

### 🔒 Security

- Migrate to bouncy castle

## [v2.0.0] - 2024-03-01

BREAKING CHANGE: Updated service to .NET 8 LTS.

### :arrows_counterclockwise: Changed

- update to dotnet 8
- update ci/cd templates

### :lock: Security

- apply patch policy

## [v1.4.2] - 2023-12-01

### 🆕 Added

- Add test cases for invalid receivers, senders and versions

### 🔄 Changed

- Move test cases for CryptoFileBuilder into separate file
- Declare dummy test files as binary files
- Add explicit tests for encryption and decryption methods

### 🔒 Security

- Validate signature before processing file

## [v1.4.1] - 2023-09-15

### 🔒 Security

- Extend file decryptor to validate the id of the sender certificate.

## [v1.4.0] - 2023-08-09

### 🆕 Added

- Deterministic build and include sha256 of binary into nuget package

## [v1.3.5] - 2023-04-19

### 🆕 Added

- Added attachment stations to contest configuration

## [v1.3.4] - 2023-03-31

### 🎉 Initial release for Bug Bounty
