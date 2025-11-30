# Настройка GitHub Pages для Wave Language

Этот документ содержит инструкции по развёртыванию документации Wave Language на GitHub Pages.

## Шаги по настройке

### 1. Убедитесь, что все файлы документации на месте

В папке `docs/` должны быть следующие файлы:
- `index.html` - главная страница
- `debugger.html` - визуальный отладчик
- `LANGUAGE_GUIDE.md` - полное руководство
- `TUTORIAL.md` - учебник
- `REFERENCE.md` - справочник
- `SYNTAX.md` - спецификация синтаксиса
- `_config.yml` - конфигурация Jekyll

### 2. Настройка репозитория на GitHub

1. Перейдите в настройки вашего репозитория на GitHub
2. Выберите раздел "Pages" в левом меню
3. В разделе "Source" выберите:
   - **Branch**: `main` (или `master`)
   - **Folder**: `/docs`
4. Нажмите "Save"

### 3. Ожидание развёртывания

GitHub Pages автоматически развернёт сайт в течение нескольких минут.
Ваша документация будет доступна по адресу:

```
https://<username>.github.io/<repository-name>/
```

Например: `https://yourusername.github.io/FP_WaveLang/`

### 4. Проверка работы

После развёртывания проверьте следующие страницы:

- **Главная страница**: `https://yourusername.github.io/FP_WaveLang/`
- **Отладчик**: `https://yourusername.github.io/FP_WaveLang/debugger.html`
- **Руководство**: `https://yourusername.github.io/FP_WaveLang/LANGUAGE_GUIDE.html`
- **Учебник**: `https://yourusername.github.io/FP_WaveLang/TUTORIAL.html`
- **Справочник**: `https://yourusername.github.io/FP_WaveLang/REFERENCE.html`

### 5. Обновление ссылок в README.md

Не забудьте обновить ссылки на GitHub в файле `docs/index.html` и `README.md`:

Замените `yourusername` на ваше имя пользователя GitHub:
```html
<a href="https://github.com/yourusername/FP_WaveLang" class="github-link">View on GitHub</a>
```

## Возможные проблемы и решения

### Проблема: Сайт не отображается

**Решение**: 
- Подождите 5-10 минут после настройки
- Проверьте, что папка `docs/` находится в корне репозитория
- Убедитесь, что файл `index.html` существует

### Проблема: 404 ошибка на страницах

**Решение**:
- Убедитесь, что все файлы `.md` и `.html` находятся в папке `docs/`
- Проверьте регистр в именах файлов (например, `TUTORIAL.md`, а не `tutorial.md`)

### Проблема: Стили не применяются

**Решение**:
- Стили встроены в HTML файлы, поэтому должны работать сразу
- Проверьте консоль браузера на наличие ошибок

## Локальная проверка

Для проверки сайта локально используйте простой HTTP сервер:

### С помощью Python:
```bash
cd docs
python -m http.server 8000
```

Затем откройте `http://localhost:8000` в браузере.

### С помощью Node.js:
```bash
cd docs
npx http-server
```

## Автоматическое развёртывание

GitHub Pages автоматически обновляет сайт при каждом push в ветку `main`.
Изменения появляются в течение 1-5 минут после push.

## Дополнительные настройки

### Кастомный домен

Если у вас есть собственный домен:
1. Создайте файл `CNAME` в папке `docs/`
2. Добавьте в него ваш домен: `wave-lang.example.com`
3. Настройте DNS записи у вашего провайдера

### HTTPS

GitHub Pages автоматически предоставляет HTTPS для сайтов.
Для включения принудительного HTTPS:
1. Перейдите в Settings → Pages
2. Установите галочку "Enforce HTTPS"

---

*Если у вас возникли проблемы с настройкой GitHub Pages, обратитесь к [официальной документации](https://docs.github.com/en/pages).*

