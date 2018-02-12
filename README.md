# Laughing Cilantro

Personal finance statistician. Import your past transactions and see the pretty graphs and stats! (Doesn't rely on live connections to your bank accounts, and/or your secure banking credentials.)

This project is intended as a quick-and-dirty learning app. It uses an [onion architecture](http://jeffreypalermo.com/blog/the-onion-architecture-part-3/), which may strike you as strange if you're used to "traditional" three-tier apps (MVC/web, business logic, and data access).

It also skips out on several important facets that should be present in any production web application, including:

- A separate web-based API (eg. WebAPI, ServiceStack)
- Unit tests (including Javascript/Angular unit tests)
- Functional tests (browser calls/workflows)
- Centralized, incremental database migrations
