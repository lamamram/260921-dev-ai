# AGENTS.md

Laboratoire de formation **OpenCode + Docker Sandboxes** (ce dossier est monté dans le sandbox).
Docs, commentaires et modèles du dépôt sont en **français** : écrire tout texte ajouté en français.

## Arorescence Projet

- `example/ECommerce.Api/` — seule application : minimal API ASP.NET Core 10, tout dans `Program.cs`,
  données en mémoire (aucune base de données), modèles en `record` dans `Models/`. **Aucun test.**
- `example/kit/spec.yaml` — kit sandbox (mixin) qui installe le SDK .NET **à la création** du sandbox.
  Pas de Dockerfile dans ce dépôt.
- `.opencode/opencode.jsonc` — config OpenCode (il n'y a pas de `opencode.json` à la racine).
  Toute modification de config nécessite un redémarrage d'OpenCode.
- `example/ECommerce.Tests/` — projet de tests xUnit minimal, avec FluentAssertions et NSubstitute.

## Commandes

SDK .NET 10 installé par le kit (`dotnet` → `/usr/local/bin/dotnet`, `DOTNET_ROOT=/opt/dotnet`).

```bash
cd example/ECommerce.Api
dotnet build                 # seule vérification automatique du dépôt (doit passer sans erreur)
dotnet run --urls http://0.0.0.0:5000
```

```bash
cd example/ECommerce.Tests
dotnet test                  # exécute les tests unitaires xUnit
```

- Le bind **`0.0.0.0` est obligatoire** : un serveur en `127.0.0.1` est invisible via la
  publication de ports (détails dans `../AGENTS.md`).
- Aucun lint / typecheck / tests / CI ici. Vérification = `dotnet build` + smoke test :

```bash
curl http://localhost:5000/products
curl -X POST http://localhost:5000/cart/items -H "Content-Type: application/json" -d '{"productId":1,"quantity":2}'
curl http://localhost:5000/cart
curl -X POST http://localhost:5000/orders/checkout
```
