[![CI](https://github.com/GeoWerkstatt/interlis-model-browser/actions/workflows/ci.yml/badge.svg)](https://github.com/GeoWerkstatt/interlis-model-browser/actions/workflows/ci.yml)
[![Release](https://github.com/GeoWerkstatt/interlis-model-browser/actions/workflows/release.yml/badge.svg)](https://github.com/GeoWerkstatt/interlis-model-browser/actions/workflows/release.yml)

# INTERLIS Modell-Suche («INTERLIS Model Browser»)

Webapplikation zur Suche von INTERLIS-Modellen in öffentlichen Repositories mittels Eigenschaften wie Name, Dateiname, publizierende Stelle/Kanton oder Modell-Version.

## Einrichten der Entwicklungsumgebung

Folgende Komponenten müssen auf dem Entwicklungsrechner installiert sein:

* Git
* Docker
* Visual Studio 2022 oder Visual Studio Code
* .NET 8
* Node.js 16 LTS

## Neue Version erstellen

Ein neuer GitHub _Pre-release_ wird bei jeder Änderung auf [main](https://github.com/GeoWerkstatt/interlis-model-browser) [automatisch](./.github/workflows/pre-release.yml) erstellt. In diesem Kontext wird auch ein neues Docker Image mit dem Tag _:edge_ erstellt und in die [GitHub Container Registry (ghcr.io)](https://github.com/geowerkstatt/interlis-model-browser/pkgs/container/interlis-model-browser) gepusht. Der definitve Release erfolgt, indem die Checkbox _This is a pre-release_ eines beliebigen Pre-releases entfernt wird. In der Folge wird das entsprechende Docker Image in der ghcr.io Registry mit den Tags (bspw.: _:v1.2.3_ und _:latest_) [ergänzt](./.github/workflows/release.yml).
