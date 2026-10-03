# TaskManager API

REST API для управления задачами, написанное на ASP.NET Core.

**Живое демо:** https://taskmanagerwow-1.onrender.com/swagger/index.html

> Бесплатный тариф Render может "засыпать" при простое — первый запрос после паузы может занять 30-60 секунд на холодный старт.

## Стек технологий
- ASP.NET Core 9, C#
- Entity Framework Core + SQLite
- JWT-аутентификация (регистрация, логин, BCrypt)
- AutoMapper
- FluentValidation
- Serilog
- xUnit + Moq (unit-тесты контроллера и сервисного слоя)
- Docker (multi-stage build)

## Архитектура
- Контроллеры → сервисный слой (`ITaskService`/`TaskService`) → EF Core
- Разделение по проектам: `TaskManager.Api`, `TaskManager.Domain`, `TaskManager.Infrastructure`, `TaskManager.Tests`

## Функциональность
- CRUD для задач, привязанных к авторизованному пользователю
- JWT-аутентификация: регистрация, логин, защита эндпоинтов
- Фильтрация по статусу и дате создания, поиск по названию
- Сортировка и пагинация
- Валидация входных данных
- Централизованная обработка ошибок и логирование
- Unit-тесты на бизнес-логику

## Как запустить локально
\`\`\`bash
git clone https://github.com/kakakakakakakakakak0atm3a1/TaskManagerWOW.git
cd TaskManagerWOW
dotnet restore
dotnet ef database update --project TaskManager.Infrastructure --startup-project TaskManager.Api
dotnet run --project TaskManager.Api
\`\`\`
Открой `http://localhost:5220/swagger`

## Как запустить через Docker
\`\`\`bash
docker-compose up --build
\`\`\`

## API Endpoints
- `POST /api/auth/register` — регистрация
- `POST /api/auth/login` — логин, возвращает JWT
- `GET /api/tasks` — список задач (фильтры: isDone, createdAfter, search, sortBy, page, pageSize)
- `GET /api/tasks/{id}` — задача по id
- `POST /api/tasks` — создать задачу
- `DELETE /api/tasks/{id}` — удалить задачу

## Тесты
\`\`\`bash
dotnet test
\`\`\`