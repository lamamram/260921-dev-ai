## lire des produits en fonction de catégories

* route: `GET /products?category=...`

* exemple: `GET /products?category=informatique` → retourne tous les produits de la catégorie "informatique"


### tasks

1. ajouter un paramètre `category` au model Product pour filtrer les produits par catégorie.

2. ajouter les valeurs de catégories ci-dessous dans Programs.cs pour chaque produit:

   - Clavier mécanique → catégorie: "informatique"
   - Souris sans fil → catégorie: "informatique"
   - Écran 27" 4K → catégorie: "informatique"
   - Casque audio → catégorie: "audio"

3. modifier la route `GET /products` pour qu'elle accepte un paramètre optionnel `category` de type Query Params. Si ce paramètre est fourni, la route doit retourner uniquement les produits de cette catégorie. Sinon, elle doit retourner tous les produits.