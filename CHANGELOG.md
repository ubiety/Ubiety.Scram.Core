# Change Log

All notable changes to this project will be documented in this file. See [versionize](https://github.com/saintedlama/versionize) for commit guidelines.

<a name="2.0.0"></a>
## [2.0.0](https://www.github.com/ubiety/Ubiety.Scram.Core/releases/tag/v2.0.0) (2025-03-11)

### Features

* add attribute for gs2 header ([24be987](https://www.github.com/ubiety/Ubiety.Scram.Core/commit/24be987c5a50ed178ad1e067ed2464302402b089))
* add channel binding with token added to messages for tls-unique. closes [#7](https://www.github.com/ubiety/Ubiety.Scram.Core/issues/7), [#5](https://www.github.com/ubiety/Ubiety.Scram.Core/issues/5), [#6](https://www.github.com/ubiety/Ubiety.Scram.Core/issues/6) ([8e8bc8e](https://www.github.com/ubiety/Ubiety.Scram.Core/commit/8e8bc8efbde7e6156e3808264b98a18ad493cf29))
* add implicit conversion to client final message ([db86ac4](https://www.github.com/ubiety/Ubiety.Scram.Core/commit/db86ac46c45a92d2aa3c69d661a8b55bb7c0b62e))
* add implicit conversions for server messages ([b3903ff](https://www.github.com/ubiety/Ubiety.Scram.Core/commit/b3903ff80128dd6f0e02d0812a275c0e59dab690))
* add index page ([25edfb1](https://www.github.com/ubiety/Ubiety.Scram.Core/commit/25edfb139ca932700aa30514a40a279582026ad9))
* add SHA512 hashes ([abd0bd8](https://www.github.com/ubiety/Ubiety.Scram.Core/commit/abd0bd8a2151c1bb30f34448a45cae73ef0f6ce8))
* add the bare received message to first server message for proof calculation ([bc5af5d](https://www.github.com/ubiety/Ubiety.Scram.Core/commit/bc5af5db75807a0943a66dfbf74ad8260dfa3cc6))
* add tls version enum ([c40f167](https://www.github.com/ubiety/Ubiety.Scram.Core/commit/c40f1677829339e45cf537c5e72de988dacca81e))
* add tls version to the first message and gs2 attribute so type of channel binding can be specified. ([41984d7](https://www.github.com/ubiety/Ubiety.Scram.Core/commit/41984d73a1bd1ccd66264467c715725ab7776e43))
* calculate proof for final message when created ([9141216](https://www.github.com/ubiety/Ubiety.Scram.Core/commit/9141216ef89de96aa4108eb215eaf8b9777a5024))
* GS2 headers and parsing of the first client message ([fe75d86](https://www.github.com/ubiety/Ubiety.Scram.Core/commit/fe75d86189c8c745e0222a9b9bedc6eec1825d16))
* implement exception constructors according to pattern ([db5be5b](https://www.github.com/ubiety/Ubiety.Scram.Core/commit/db5be5be85cbb601c93ef48f3252556ca206a4d7))
* implement IEquatable properly ([8200278](https://www.github.com/ubiety/Ubiety.Scram.Core/commit/82002786114faa676e69396c70264e8d9818475e))
* implement implicit conversions for first client message ([55d562c](https://www.github.com/ubiety/Ubiety.Scram.Core/commit/55d562c474c4ae087fe8d322484a0ea1ef5320ec))
* parse scram messages with regex ([2abc3af](https://www.github.com/ubiety/Ubiety.Scram.Core/commit/2abc3af25df61a1672fb13ac72a379ca04bb65e1))
* upgrade to .NET 9.0 and modernize build pipeline ([c36cac9](https://www.github.com/ubiety/Ubiety.Scram.Core/commit/c36cac9b203ef5ecdd686d5f2df9cad80807e40b))
* **test:** add testing project ([506d06d](https://www.github.com/ubiety/Ubiety.Scram.Core/commit/506d06d79abb5fa2a0083bde53d99144b5e9cb98))

### Bug Fixes

* add tool frameworks ([59c92eb](https://www.github.com/ubiety/Ubiety.Scram.Core/commit/59c92ebe41aaf8a6fd1af85b8c4b6f7cb7fdad5f))
* flip for loop increment operation ([60b4577](https://www.github.com/ubiety/Ubiety.Scram.Core/commit/60b4577a228d5942ab5506634113ee8947601fba))
* issue with channel attribute header and tests ([11c2f3b](https://www.github.com/ubiety/Ubiety.Scram.Core/commit/11c2f3bfc6cb64ab6da39a06c472c782b595fd01))
* make docs project unpackable ([6625e36](https://www.github.com/ubiety/Ubiety.Scram.Core/commit/6625e36e36d016918012151da28a742151e963ee))
* remove docs project and docfx install ([df6503e](https://www.github.com/ubiety/Ubiety.Scram.Core/commit/df6503e7975a9af7e5dba761c23575b0ca1cd118))
* resolve code smells for nullable types ([13d2f6a](https://www.github.com/ubiety/Ubiety.Scram.Core/commit/13d2f6aedb4b978b3393c235511a08359b7212d3))
* set appveyor java_home ([7817465](https://www.github.com/ubiety/Ubiety.Scram.Core/commit/78174650f5b67ecc3b961d9bf81bfdce641f06e3))
* update gitversion branch regex ([cf8f735](https://www.github.com/ubiety/Ubiety.Scram.Core/commit/cf8f735721f75aae7592378554744271683282e2))
* update gitversion config formatting ([4cbd398](https://www.github.com/ubiety/Ubiety.Scram.Core/commit/4cbd3980de8d7b0ee2c866f55bf293e6df69b0d0))
* update gitversion to recognize main branch ([27a1b7f](https://www.github.com/ubiety/Ubiety.Scram.Core/commit/27a1b7f060644f48689e22704b2311e0393252be))
* update icon path ([859f5ed](https://www.github.com/ubiety/Ubiety.Scram.Core/commit/859f5ed1d86bc610bd64a023d0d051cf6e5b579c))
* update project language version ([14ca5bc](https://www.github.com/ubiety/Ubiety.Scram.Core/commit/14ca5bcb20affc9af842e47d38dc1920abf4d687))
* update sonar scanner build tasks ([a92baa6](https://www.github.com/ubiety/Ubiety.Scram.Core/commit/a92baa60c72d8804fbab78f171e55a0b50e4da00))
* **hash:** fix an error in calculating the password hash ([4d0daee](https://www.github.com/ubiety/Ubiety.Scram.Core/commit/4d0daee78bc659297a3793f3dc7fd2b73e7d3075))

### Breaking Changes

* move messages to message namespace ([1ce62de](https://www.github.com/ubiety/Ubiety.Scram.Core/commit/1ce62de976b83003ac1927f2b306a37781497b6b))
* upgrade to .NET 9.0 and modernize build pipeline ([c36cac9](https://www.github.com/ubiety/Ubiety.Scram.Core/commit/c36cac9b203ef5ecdd686d5f2df9cad80807e40b))

<a name="1.1.0"></a>
## 1.1.0 (2019-1-27)

### Bug Fixes

* fix an error in calculating the password hash

### Features

* add testing project

## 1.0.0 (2019-1-26)

### Breaking Changes

* move messages to message namespace

## 0.0.2 (2019-1-26)

### Bug Fixes

* flip for loop increment operation

## 0.0.1 (2019-1-25)

