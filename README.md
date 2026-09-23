## Service-grænser 

### InventoryService - costum lagerservice som krav fra opgaven
### CatalogService
### PaymentService
Disse grænser valideres med cohesion, coupling og information hiding.
Bruger DDD til at finde og fastholde servicegrænserne i form af bounded contexts
- Ideen er at software skal afspejle hvordan forretningen tænker og arbejder.
F.eks. hvis en service vil "ændre" noget i CatalogService, skal den gå igeenm CatalogService som selv ændre det i sin egen database.

## Mono repo vs Multi repo - CI/CD - Brancing-style -- Deployment style
- Begrund jeres valg baseret på teamstørrelse og service dependencies - 
Jeg vil bruge et mono-repo, da teamets størrelse er 1. Med kun én udvikler er jeg ikke
afhængig af andres ændringer, og den koordination et multi-repo skal løse, findes ikke her.

- Dokumenter hvordan I håndterer cross-service changes - 
Hvis der skulle komme crossservice changes, vil min CI/CD builde og deploye begge baseret på path-ændringer, jeg bør dog overveje om min servicegrænse egentlig er sat korrekt, hvis dette sker ofte. 

**Branching:** Jeg bruger trunk-based development, hvor små ændringer løbende merges direkte til main. Når en service ændres, bliver kun den bygget, testet og deployet. Det kræver dog, at CI/CD-pipelinen er sat ordentligt op, så den kun reagerer på ændringer i den berørte service. Ellers vil selv en lille ændring udløse et genbyg af alle services.
- Fordele: Mindre merge-konflikter, CI køres ved hver ændring hver lille commit(fejl opfanges tidligt), små ændringer kan gå i produktion med det samme.

**CI/CD:** Én GitHub Actions-workflow pr. deployerbar enhed, path-filtreret så kun den
ændrede service bygges. Ændringer i `shared/` udløser alle services.

- **`_dotnet-service.yml`** er selve pipelinen. Den kører ikke af sig selv (`workflow_call`),
  men tager servicenavnet som input og udleder alle stier fra det.
- **`catalog.yml`** er den konkrete service. Den definerer path-filteret og kalder
  pipelinen med `service: Catalog`. Inventory og Payment bliver kopier med ét ord ændret. 
  (Dette workflow køres hvis: der er ændringer i en service + push til main (som følger vores branching-strategi))

Stages: `build → fast-tests → slow-tests → Artifact Creation → Enviromental Deployment: staging/production`.
Fast tests er unit tests med `timeout-minutes: 2` (krav fra opgaven), slow tests er integrationstests. 
Vi genbruger buildet fra build-stage som gør vi overholder: "build once, deploy everywhere" princippet"

Fordi stierne udledes af servicenavnet, ejer hver service sin egen test-suite:
```
services/<service>/tests/<service>.UnitTests/
services/<service>/tests/<service>.IntegrationTests/
```
`monolit.yml` er det oprindelige CI-workflow fra eShopOnWeb, som bygger og tester hele
solutionen. Den er indtil videre den eneste aktive pipeline, da `services/` er tom.
For hver strangler-cyklus skrumper dens path-filter, og til sidst kan den fjernes helt.

## Strangler Fig Pattern implementering - funktionalitet flyttes gradvist ud uden driftafbrydelse

### Først CatalogService
xxxx



