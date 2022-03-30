# MyLab.TaskKicker

Ознакомьтесь с последними изменениями в [журнале изменений](./CHANGELOG.md).

## Обзор

`MyLab.TaskKicker` - сервис, предназначенный для запуска задач на основе [MyLab.TaskApp](https://github.com/mylab-task/task-app) по расписанию. 

В общем случае отправляет `HTTP` в соответствии с настройками.

 ## Настройки

### `appsettings.json`

Содержит стандартные настройки `.NET` приложения. Такие, как [настройки логирования](https://docs.microsoft.com/en-us/dotnet/core/extensions/logging?tabs=command-line#configure-logging).

### `jobs.yml`

Содержит настройки работ по запуску задач.

Пример содержания файла:

```yaml
jobs:
- id: 'foo'
  cron: '* * * * * ? *'
  host: 'localhost'
  path: '/path'
  port: 8080
  headers: 
    foo-key: foo-val
    bar-key: bar-val
- id: 'bar'
  host: 'bar-host'
```

Структура файла:

* `jobs` - список задач:
  * `id` - идентификатор задачи. Фигурирует в логах. По умолчанию назначается случайный `GUID`;
  * `cron` - строка с расписанием в формате [cron/quartz](https://www.freeformatter.com/cron-expression-generator-quartz.html). По умолчанию `0 * * * *`.
  * `host` - хост адреса запроса Таска. Обязательный параметр;
  * `path` - путь адреса запроса Таска. `/processing` - по умолчанию;
  * `port`- порт адреса запроса Таска. `80` - по умолчанию;
  * `headers` - список дополнительных заголовков запроса. Опционально.

## Развёртывание

Сервис развёртывается на базе `docker` контейнера. Пример `docker-compose` файла:

```yaml
version: '3.3'

services:
  task-kicker:
    container_name: task-kicker
    image: docker pull ghcr.io/mylab-task/task-kicker:latest
    volumes:
    - ./jobs.yml:/app/jobs.yml
```

Конфигурационные файлы в контейнере:

* `/app/appsettings.json`

* `/app/jobs.yml`
