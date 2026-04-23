# Honey & Bunny

The main implementation developed as part of this diploma project is concentrated in:

- [`Assets/Scripts/Diploma`](https://github.com/KatTihanovich/Honey-Bunny-diploma/tree/main/Assets/scripts/Diploma)
- [`Assets/Scripts/Enemy/FSM`](https://github.com/KatTihanovich/Honey-Bunny-diploma/tree/main/Assets/scripts/Enemy/FSM)
---
## Service Availability

The backend services are currently not deployed due to infrastructure cost limitations.

However, the full server-side implementation is included in repositories [game-progress-api](https://github.com/KatTihanovich/game-progress-api) and [game-progress-db](https://github.com/KatTihanovich/game-progress-db) :
- REST API (Java Spring Boot)
- Database integration (PostgreSQL)
- CI/CD configuration (GitHub Actions, Docker)

The system can be deployed locally or on external infrastructure if needed.

---

**Honey & Bunny** is a cross-platform 2D platformer with narrative and psychological elements, developed in **Unity**.

The project focuses on building a stable, scalable, and maintainable technical architecture that combines gameplay systems, physics simulation, and interaction with remote server infrastructure.

---

## Project Goals

- Develop predictable and controllable AI agent behavior.
- Implement realistic physics simulation of environment objects, including a rope system.
- Ensure reliable saving and loading of game progress via REST API and PostgreSQL.
- Set up an automated CI/CD pipeline for the server-side.
- Provide stable cross-platform builds for **Windows** and **macOS**.

---

## Core Subsystems

### Game Agents
- Implemented using **FSM / HFSM** architectures.
- Uses the **Blackboard pattern** for coordination and synchronization.

### Physics System
- Handles physical simulation of interactive environment objects.
- Includes a dedicated **rope system** for character interaction.

### Network & Persistence
- Unity client communicates with a remote REST API.
- Stores player progress and statistics in **PostgreSQL**.

### Server-Side
- Built with **Java Spring Boot**.
- Provides:
  - progress saving/loading;
  - achievements tracking;
  - player statistics calculation.

---

## Technologies Used

| Area | Technology |
|------|-----------|
| Game Engine | Unity |
| AI Behavior | FSM / HFSM, Blackboard |
| Physics | Unity Physics |
| Backend | Java Spring Boot |
| Database | PostgreSQL |
| CI/CD | GitHub Actions, Docker |
