# Honey & Bunny

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
