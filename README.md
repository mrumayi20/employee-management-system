## Layers

1. Domain: pure business entities + rules (no EF, no web, no MySQL)

2. Application: use-cases (commands/queries), validation, DTOs, interfaces

3. Infrastructure: MySQL/EF Core, repositories, file/PDF/Excel services

4. API: controllers/endpoints, auth wiring, DI, swagger

5. Frontend stays separate and talks to API over HTTP.

Why: keeps business rules independent of frameworks/UI/DB; easier testing; easier growth (bonus analytics won’t become a mess).

1. API depends on Application + Infrastructure

2. Infrastructure depends on Application

3. Application depends on Domain
