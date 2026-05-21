# AutoEmpiric

Framework d'orchestration et de validation empirique pour agents autonomes.

## Architecture

AutoEmpiric repose sur une architecture polyglotte :

- **C#** : coeur de l'orchestration, API HTTP et contrats de validation.
- **Python** : logique agent, calibration de confiance et intégrations analytiques.
- **Rust** : composants de sandboxing et d'isolation d'exécution.

## Prérequis

- .NET 10.0 SDK
- Python 3.12
- Cargo/Rust, uniquement pour le composant `AutoEmpiric.Sandbox.Rust`

## Vérification

```bash
dotnet test AutoEmpiric.sln
python -m compileall -q src/AutoEmpiric.Agents.Python/autoempiric_agents src/AutoEmpiric.Agents.Python/tests
python -m pytest src/AutoEmpiric.Agents.Python/tests
cd src/AutoEmpiric.Sandbox.Rust && cargo test
```

## Lancement de l'API

```bash
dotnet run --project src/AutoEmpiric.API
```

Endpoints disponibles :

- `GET /api/health`
- `POST /api/mission/submit` avec un JSON `{ "problemDescription": "..." }`

## Configuration

L'exécution peut utiliser la variable d'environnement optionnelle suivante :

- `MAX_REASONING_STEPS`

## Structure des répertoires

- `src/AutoEmpiric.Core` : composants de base pour l'orchestration et la validation.
- `src/AutoEmpiric.API` : API HTTP ASP.NET Core.
- `src/AutoEmpiric.Agents.Python` : agents Python et calibration de confiance.
- `src/AutoEmpiric.Sandbox.Rust` : sandbox Rust.
- `tests/AutoEmpiric.Core.Tests` : tests unitaires du coeur C#.
