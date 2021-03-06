# Simulations des retraites en France

L'objectif est d'abordé la réforme des retraites de 2019/2020 sur la base des "détails techniques" systématiquement évités/oubliés par tous les protagonistes (que ce soit les opposants ou les soutients). Et pourtant c'est par ce biais qu'on peut réellement tirer des conclusions, et tirer le vrai du faux.

Contrairement à ce qui est couramment dit, les documents suivants comportent suffisamment de détails sur les paramètres envisagés pour arriver à simuler les retraites :

- [Rapport Delevoye de septembre 2019](https://reforme-retraite.gouv.fr/IMG/pdf/retraite_01-09_leger.pdf)
- [Communiqué de presse du 1er ministre du 11 décembre 2019](https://reforme-retraite.gouv.fr/IMG/pdf/dossier_de_presse_-_systeme_universel_de_retraite_-11.12.2019.pdf)

## Ce simulateur VS les débats sur la réforme

Voici les différents sujets qu'on peut analyser ou non avec ce simulateur :

- Comparaison des régimés spéciaux/autonomes avec la réforme : ne contient que le régimé général des salariés, la SSI (indépendants commerçants/artisans), la CIPAV (certaines professions libérales) et les régimes cibles (salariés et indépendants - oui il y a un aménagement des cotisations pour les indépendants)
  - [Comparaison des régimes 1) salarié actuel et 2) réformé](./analyse/comparaison-salaries.md)
  - [Comparaison des régimes pour les indépendants : 1) SSI, 2) CIPAV, 3) réfomé](https://www.kickbanking.com/blog/la-reforme-des-retraites-pour-les-independants.html)
- [Age pivot/d'équilibe]() : ces 2 termes désignent le même principe proche de l'âge du taux plein d'aujourd'hui (décôte en dessous, surcôte au dessus), pour le moment le simulateur prend en compte la proposition initiale (en attendant plus de clarification sur "l'abandon")
- [Carrière longue]() : pris en compte sur la base du rapport Delevoye
- [Pénibilité]() : ce n'est pas pris en compte pour le moment
- [Retraite minimum]() : ce n'est pas pris en compte pour le moment
- [Enfants]() : ce n'est pas pris en compte pour le moment

## Lancer les simulations soi-même

Le code est en F#, les prérequis sont [ceux décrits ici (cliquer sur Use et choisissez votre plateforme)](https://fsharp.org/).

Le F# ayant un mode script (REPL appelé FSI pour FSharp Interactive), le point d'entrée est le script `retraites.fsx`. Depuis l'IDE (VSCode + Ionide par ex) que vous avez choisi, il suffit de sélectionner les lignes à envoyer dans FSI pour les exécuter (on peut aussi exécuter le script via la ligne de commande `fsi retraites.fsx`). Le script contient les appels aux fonctions de calcul définies dans les différents fichier `.fs`, ainsi que la mise en forme de graphique Google Charts (via XPlot).
