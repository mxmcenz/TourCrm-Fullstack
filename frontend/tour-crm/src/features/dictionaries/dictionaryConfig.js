export const dictionaries = {
    countries: {
        title: 'Страны',
        endpoint: 'country',
        fields: [
            { key: 'name', label: 'Название', required: true }
        ]
    },
    cities: {
        title: 'Города',
        endpoint: 'city',
        fields: [
            { key: 'name', label: 'Название', required: true },
            { key: 'countryId', label: 'Страна', type: 'select', required: true, source: 'country' }
        ]
    },
    servicetypes: {
        title: 'Типы услуг',
        endpoint: 'serviceType',
        fields: [
            { key: 'name', label: 'Название', required: true }
        ]
    },
    numbertypes: {
        title: 'Типы номеров',
        endpoint: 'numberType',
        fields: [
            { key: 'name', label: 'Название', required: true }
        ]
    },
    accommodationtypes: {
        title: 'Типы размещения',
        endpoint: 'accommodationType',
        fields: [
            { key: 'name', label: 'Название', required: true }
        ]
    },
    mealtypes: {
        title: 'Типы питания',
        endpoint: 'mealType',
        fields: [
            { key: 'name', label: 'Название', required: true }
        ]
    },
    partners: {
        title: 'Партнеры',
        endpoint: 'partner',
        fields: [
            { key: 'name', label: 'Название', required: true }
        ]
    },
    touroperators: {
        title: 'Туроператоры',
        endpoint: 'tourOperator',
        fields: [
            { key: 'name', label: 'Название', required: true }
        ]
    },
    citizenships: {
        title: 'Гражданства',
        endpoint: 'citizenship',
        fields: [
            { key: 'name', label: 'Название', required: true }
        ]
    },
    partnermarks: {
        title: 'Метки партнеров',
        endpoint: 'partnerMark',
        fields: [
            { key: 'name', label: 'Название', required: true }
        ]
    },
    partnertypes: {
        title: 'Типы партнеров',
        endpoint: 'partnerType',
        fields: [
            { key: 'name', label: 'Название', required: true }
        ]
    },
    currencies: {
        title: 'Валюты',
        endpoint: 'currencies',
        fields: [
            { key: 'name', label: 'Название', required: true }
        ]
    },

    labels: {
        title: 'Метки',
        endpoint: 'labels',
        fields: [
            { key: 'name', label: 'Название', required: true }
        ]
    },

    leadrequesttypes: {
        title: 'Типы заявок',
        endpoint: 'leadRequestTypes',
        fields: [
            { key: 'name', label: 'Название', required: true }
        ]
    },

    leadsources: {
        title: 'Источники заявок',
        endpoint: 'leadSources',
        fields: [
            { key: 'name', label: 'Название', required: true }
        ]
    },

    leadstatuses: {
        title: 'Статусы лидов',
        endpoint: 'leadStatuses',
        fields: [
            { key: 'name', label: 'Название', required: true }
        ]
    },

    hotels: {
        title: 'Отели',
        endpoint: 'hotels',
        fields: [
            { key: 'name',   label: 'Название', required: true },
            { key: 'cityId', label: 'Город',    type: 'select', required: true, source: 'city' },
            { key: 'stars',  label: 'Звезды' }
        ]
    },
    visatypes: {
        title: 'Типы виз',
        endpoint: 'visaTypes',
        fields: [
            { key: 'name', label: 'Название', required: true }
        ]
    },
    dealstatuses: {
        title: 'Статусы сделок',
        endpoint: 'dealStatuses',
        fields: [
            { key: 'name',  label: 'Название', required: true },
            { key: 'isFinal', label: 'Финальный', type: 'checkbox' }
        ]
    },
}
