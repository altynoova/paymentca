# PaymentsCA
API на **ASP.NET Core + PostgreSQL** с авторизацией и платежами.

## Запуск
docker-compose up --build

- API → [http://localhost:8080/swagger]  

## Эндпоинты

- `POST /login` → логин, выдаёт токен  
- `POST /logout` → инвалидирует токен  
- `POST /charge` → списывает $1.10, возвращает новый баланс  

### Тестовый пользователь
"username": "user",
"password": "abc1234"