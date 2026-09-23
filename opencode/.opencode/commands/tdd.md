---
description: "[ spec_name ] lance une procédure de TDD pour une spécification en paramètre"
agent: build
model: opencode/big-pickle
---

si $1 est vide, ou si le fichier `$1.md` n'existe pas dans le dossier de specs, la commande s'arrête et affiche un message

* utilise la skill `/test-driven-development` pour lancer une procédure de TDD sur la spécification fournie dans le fichier `$1.md` du dossier de specs.

* dans le cadre de cette procédure, utiliser l'agent @cs-tester pour écrire les tests unitaires et d'intégration dans le projet de tests du projet et lancer les tests RED et GREEN, et utiliser l'agent courant pour écrire le code de la fonctionnalité.
