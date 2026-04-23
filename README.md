# Запуск проекта ✈️

## Подготовка окружения
1. В корне проекта лежит файл-шаблон **.env.example**. Скопируйте его в **.env** и укажите только данные для БД: `cp .env.example .env`. Измените при необходимости `POSTGRES_USER` и `POSTGRES_PASSWORD`.
2. В Git репозитории коммитим **только .env.example**. Локальные файлы `.env.development` и серверные `.env.production` игнорируются.

---

## Запуск локально через Docker
1. Перейти в корень проекта: `cd C:\Users\...\...\tourcrm\ac14-team3`
2. Остановить старые контейнеры (если висят): `docker compose down -v`
3. Запустить в фоне с использованием `.env`: `docker compose --env-file .env up -d --build`  
   ⚠️ Первая сборка может занять много времени. Если упадёт фронт из-за несоответствия зависимостей, выполните: `cd frontend/tour-crm && npm install && cd ../.. && docker compose --env-file .env up -d --build`
4. Проверка работы: Фронт: [http://localhost/](http://localhost/) • Бэк Swagger: [http://localhost:8080/swagger](http://localhost:8080/swagger)
5. Просмотр статуса: `docker compose ps`
6. Логи сервисов: `docker compose logs -f backend` • `docker compose logs -f frontend` • `docker compose logs -f db`
7. Остановка: `docker compose down`

---

## Запуск локально без Docker (раздельно фронт и бэк)

### Backend
1. Перейти в каталог `backend/TourCrm`.
2. Убедиться, что PostgreSQL запущен локально.
3. Запустить: `dotnet run` (по умолчанию бэк слушает [http://localhost:8081](http://localhost:8081)).

### Frontend
1. Перейти в каталог `frontend/tour-crm`.
2. Установить зависимости (первый раз): `npm install`
3. Запустить дев-сервер: `npm run serve` (фронт откроется на [http://localhost:5173](http://localhost:5173), все запросы на `/api` будут проксироваться на `http://localhost:8081`).

---

## Автоматическая поставка (CI/CD → stage)
1. Все доработки заливаются в **feature-ветки**.
2. После ревью вливаются в **dev**.
3. Для выката на тестовый сервер создаётся **Merge Request из dev в stage**.
4. После merge в **stage** автоматически запускается пайплайн GitLab: Runner подключается по SSH к серверу, обновляет код (`git fetch/reset`), пересобирает и поднимает контейнеры (`docker compose up -d --build`).
5. Через 1–2 минуты изменения доступны на тестовом сервере.

⚠️ Напрямую пушить в stage нельзя (ветка защищена). Поставка идёт **только через MR dev → stage**.
