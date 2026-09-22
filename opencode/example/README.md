# Exercice : e-commerce minimal en C# (.NET)

Projet volontairement minimal pour s'exercer au développement assisté par IA
dans un [Docker Sandbox](https://docs.docker.com/ai/sandboxes/) (microVM). Il
n'a pas vocation à être complet : c'est un point de départ à faire évoluer
pendant les exercices.

## Contenu

- `kit/spec.yaml` : [kit sandbox](https://docs.docker.com/ai/sandboxes/customize/kits/)
  (mixin) qui installe le SDK .NET (via `dotnet-install.sh`) dans le sandbox
  `opencode`, à la place d'une image Dockerfile.
- `ECommerce.Api/` : API minimale (minimal API ASP.NET Core) avec un catalogue
  produits, un panier et des commandes, tout en mémoire (pas de base de données).

## Lancer le sandbox avec le kit .NET

Depuis `.\opencode` (le dossier monté par le sandbox) :

```bash
sbx run opencode --kit ./example/kit/ --publish 5000:5000
```

Le kit installe le SDK .NET une seule fois à la création du sandbox. Pour
relancer l'installation après une modification du kit :

```bash
sbx rm opencode && sbx run opencode --kit ./example/kit/ --publish 5000:5000
```

## Dans le sandbox : lancer l'API

```bash
cd example/ECommerce.Api
dotnet run --urls http://0.0.0.0:5000
```

## Tester

```bash
curl http://localhost:5000/products
curl -X POST http://localhost:5000/cart/items -H "Content-Type: application/json" -d '{"productId":1,"quantity":2}'
curl http://localhost:5000/cart
curl -X POST http://localhost:5000/orders/checkout
```
