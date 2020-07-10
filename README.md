# ContactPoint IP Phone - Core

![Master](https://github.com/ipphone/core/workflows/Master/badge.svg) ![CI Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=IPP&metric=alert_status)
[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2Fipphone%2Fcore.svg?type=shield)](https://app.fossa.com/projects/git%2Bgithub.com%2Fipphone%2Fcore?ref=badge_shield)

Latest binaries: https://github.com/ipphone/core/releases/latest

## Features

- Lightweight SIP softphone
- Supports Windows x32 & x64
- Highly customizable plugin architecture
  - Contacts book
  - Calls log
  - Hot keys support
  - OS-native click to call
  - Browser trigger on incoming call
  - Advanced SIP headers support for Call Centers

## Screenshots

<img src="https://ipphone.github.io/images/ipphone-04_final-08.png" width="310" /> <img src="https://ipphone.github.io/images/ipphone-02.png" />

## Configuration

SIP account configuration settings:
- **Display Name:** _anything you want_
- **Username:** _sip account username_ (without domain name part, like: 1001)
- **Password:** _sip account password_
- **Realm:** _*_ (asterisk symbol)
- **SIP Registrar:** asterisk server address (server IP or DNS name)

> **For example**, if you have SIP account `1001@asterisk.local` with password `12345678`, configuration settings you would use:
> - **Display Name:** `1001@asterisk.local`
> - **Username:** `1001`
> - **Password:** `12345678`
> - **Realm:** `*`
> - **SIP Registrar:** `asterisk.local`


## References

- [PJSIP – pjproject](https://github.com/pjsip/pjproject)
- [ComponentFactory – Krypton](https://github.com/ComponentFactory/Krypton)

## License check

[![FOSSA Status](https://app.fossa.io/api/projects/git%2Bgithub.com%2Fipphone%2Fcore.svg?type=large)](https://app.fossa.io/projects/git%2Bgithub.com%2Fipphone%2Fcore?ref=badge_large)
