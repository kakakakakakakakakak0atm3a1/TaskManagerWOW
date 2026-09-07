# TaskManager API

Простое REST API для управления задачами, написанное на ASP.NET Core.

## Стек технологий
- ASP.NET Core 9, C#
- Entity Framework Core + SQLite
- AutoMapper
- FluentValidation
- Serilog

## Функциональность
- CRUD для задач
- Фильтрация по статусу и дате создания, поиск по названию
- Сортировка и пагинация
- Валидация входных данных
- Централизованная обработка ошибок и логирование

## Как запустить
\`\`\`bash
git clone <(https://github.com/kakakakakakakakakak0atm3a1/TaskManagerWOW)>
cd TaskManager
dotnet ef database update --project TaskManager.Infrastructure --startup-project TaskManager.Api
dotnet run --project TaskManager.Api
\`\`\`
Открой `https://localhost:5220/swagger`

## API Endpoints
- `GET /api/tasks` — список задач (фильтры: isDone, createdAfter, search, sortBy, page, pageSize)
- `GET /api/tasks/{id}` — задача по id
- `POST /api/tasks` — создать задачу
- `DELETE /api/tasks/{id}` — удалить задачу