# rc6
Курсовая работа по криптографии

Алгоритм rc6 реализован в оконном приложении.
(Де-)Шифрование производится асинхронно (не допускается
отвисание UI-потока). Реализованы режимы шифрования: электронной
кодовой книги (ECB), сцепления блоков шифротекста (CBC), обратной
связи по шифротексту (CFB), обратной связи по выходу (OFB).
Есть возможность ввода ключа шифрования и вектора
инициализации для режимов шифрования. Реализован прогресс бар.