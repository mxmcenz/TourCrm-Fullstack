export const rules = {
    required: v => !!v || 'Поле обязательно',
    email: v => /.+@.+\..+/.test(v) || 'Введите корректный email',
    phone: v => !v || /^\+7\d{10}$/.test(v) || 'Формат: +7XXXXXXXXXX',
    bin: v => /^\d{12}$/.test(v) || 'Должно быть 12 цифр',
    bik: v => /^[A-Z0-9]{8}$/.test(v) || 'Должно быть 8 символов',
    iban: v => /^[A-Z0-9]{8}$/.test(v) || 'Неверный IBAN',
    code: v => /^\d{6}$/.test(v) || 'Код должен состоять из 6 цифр',
    length: v => v.length >= 6 || 'Минимум 6 символов',
}

export const matchPassword = (compareToRef) => {
    return v => v === compareToRef.value || 'Пароли не совпадают'
}
