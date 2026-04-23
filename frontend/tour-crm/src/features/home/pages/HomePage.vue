<template>
  <div class="bg-page">
    <div class="decoration-circle circle-1"></div>
    <div class="decoration-circle circle-2"></div>
    <div class="decoration-blob blob-1"></div>

    <v-container class="py-16" style="max-width:1200px">
      <v-row align="center" class="mb-16 hero-section">
        <v-col cols="12" md="6">
          <div class="hero-content">
            <h1 class="text-h3 text-ink mb-4 font-weight-bold gradient-text">
              Рабочее место<br>турагентства
            </h1>
            <p class="text-h6 text-muted mb-8" style="line-height:1.7">
              Подбирайте туры, ведите клиентов, платежи и документы —
              <span class="highlight-text">в одном элегантном пространстве</span>.
            </p>
            <div class="d-flex ga-4 align-center">
              <v-btn
                  v-if="!auth.user"
                  class="btn-pear"
                  variant="flat"
                  :ripple="false"
                  to="/register/step1"
                  size="large"
              >
                <template #prepend>
                  <v-icon icon="mdi-rocket-launch" size="20"/>
                </template>
                Начать бесплатно
              </v-btn>
              <v-btn
                  v-else
                  class="btn-pear"
                  variant="flat"
                  :ripple="false"
                  to="/company"
                  size="large"
              >
                <template #prepend>
                  <v-icon icon="mdi-arrow-right" size="20"/>
                </template>
                Перейти в систему
              </v-btn>
              <div class="trust-badge">
                <v-icon icon="mdi-shield-check" size="16" class="me-1"/>
                <span class="text-caption">Безопасно и надежно</span>
              </div>
            </div>
          </div>
        </v-col>

        <v-col cols="12" md="6">
          <div class="hero-visual">
            <v-img
                src="@/assets/hero-travel.svg"
                alt="Путешествия"
                cover
                class="hero-image"
            />
            <div class="floating-card card-1">
              <v-icon icon="mdi-passport" color="primary" size="24"/>
              <span>Визы</span>
            </div>
            <div class="floating-card card-2">
              <v-icon icon="mdi-home-city" color="primary" size="24"/>
              <span>Отели</span>
            </div>
            <div class="floating-card card-3">
              <v-icon icon="mdi-airplane" color="primary" size="24"/>
              <span>Авиа</span>
            </div>
          </div>
        </v-col>
      </v-row>

      <v-row class="mb-16 stats-section">
        <v-col cols="12" md="4">
          <v-card variant="flat" class="bg-transparent text-center py-6 stats-card">
            <div class="stat-icon-wrapper mb-4">
              <v-icon icon="mdi-timer-sand" size="40" class="text-primary"/>
            </div>
            <div class="text-h3 text-ink mb-2 font-weight-bold gradient-number">–40%</div>
            <div class="text-body-1 text-muted">меньше ручной рутины</div>
          </v-card>
        </v-col>
        <v-col cols="12" md="4">
          <v-card variant="flat" class="bg-transparent text-center py-6 stats-card">
            <div class="stat-icon-wrapper mb-4">
              <v-icon icon="mdi-chart-line" size="40" class="text-primary"/>
            </div>
            <div class="text-h3 text-ink mb-2 font-weight-bold gradient-number">+25%</div>
            <div class="text-body-1 text-muted">повторных продаж</div>
          </v-card>
        </v-col>
        <v-col cols="12" md="4">
          <v-card variant="flat" class="bg-transparent text-center py-6 stats-card">
            <div class="stat-icon-wrapper mb-4">
              <v-icon icon="mdi-laptop" size="40" class="text-primary"/>
            </div>
            <div class="text-h3 text-ink mb-2 font-weight-bold gradient-number">24/7</div>
            <div class="text-body-1 text-muted">доступ из любого места</div>
          </v-card>
        </v-col>
      </v-row>

      <div class="section-header mb-12">
        <div class="section-badge">Возможности</div>
        <h2 class="text-h4 text-ink font-weight-medium mb-4">Все что нужно для успешной работы</h2>
        <p class="text-body-1 text-muted">Интуитивные инструменты для эффективного управления вашим агентством</p>
      </div>

      <v-row class="mb-16">
        <v-col v-for="f in features" :key="f.title" cols="12" md="4">
          <v-card variant="flat" class="bg-paper pa-6 h-100 feature-card">
            <div class="feature-icon-wrapper mb-4">
              <v-icon :icon="f.icon" size="32" class="text-primary"/>
            </div>
            <div class="text-h6 text-ink mb-3 font-weight-medium">{{ f.title }}</div>
            <div class="text-body-2 text-muted" style="line-height:1.6">{{ f.text }}</div>
            <div class="feature-hover-line"></div>
          </v-card>
        </v-col>
      </v-row>

      <v-row class="mb-16 steps-section" align="center">
        <v-col cols="12" md="5">
          <div class="section-header text-left">
            <div class="section-badge">Просто начать</div>
            <h2 class="text-h4 text-ink font-weight-medium mb-4">Начните работу за 3 простых шага</h2>
            <p class="text-body-1 text-muted">Быстрая настройка и интуитивный интерфейс помогут вам начать работу сразу
              после регистрации</p>
          </div>
        </v-col>
        <v-col cols="12" md="7">
          <v-row>
            <v-col v-for="(s,i) in steps" :key="s.title" cols="12" md="4">
              <v-card variant="flat" class="bg-paper pa-5 h-100 step-card">
                <div class="step-indicator mb-4">
                  <div class="step-number">{{ i + 1 }}</div>
                </div>
                <div class="text-h6 text-ink mb-3 font-weight-medium">{{ s.title }}</div>
                <div class="text-body-2 text-muted" style="line-height:1.6">{{ s.text }}</div>
              </v-card>
            </v-col>
          </v-row>
        </v-col>
      </v-row>

      <v-card variant="flat" class="bg-paper pa-8 cta-card">
        <v-row align="center">
          <v-col cols="12" md="8">
            <div class="text-h4 text-ink mb-3 font-weight-medium">Готовы упростить работу?</div>
            <div class="text-body-1 text-muted">Подключение за 2 минуты. Бесплатно на старте. Никаких скрытых
              платежей.
            </div>
          </v-col>
          <v-col cols="12" md="4" class="text-md-right">
            <div class="d-flex flex-md-column flex-sm-row ga-3 justify-end">
              <v-btn
                  v-if="!auth.user"
                  class="btn-pear"
                  variant="flat"
                  :ripple="false"
                  to="/register/step1"
                  size="large"
              >
                Начать бесплатно
              </v-btn>
              <v-btn
                  v-else
                  class="btn-pear"
                  variant="flat"
                  :ripple="false"
                  to="/company"
                  size="large"
              >
                Открыть систему
              </v-btn>
            </div>
          </v-col>
        </v-row>
      </v-card>
    </v-container>
  </div>
</template>

<script setup>
import {useSessionStore} from '@/app/store/sessionStore'

const auth = useSessionStore()

const features = [
  {icon: 'mdi-account-group', title: 'Клиентская база', text: 'Единая карточка клиента и история поездок.'},
  {icon: 'mdi-airplane', title: 'Заявки и туры', text: 'Статусы, напоминания и документы.'},
  {icon: 'mdi-cash', title: 'Платежи', text: 'Оплаты, задолженности и акты.'},
  {icon: 'mdi-calendar', title: 'Календарь', text: 'Дедлайны виз и страховок.'},
  {icon: 'mdi-file-document', title: 'Шаблоны', text: 'Договоры и счета по шаблонам.'},
  {icon: 'mdi-shield-check', title: 'Доступы', text: 'Права сотрудников и журнал действий.'},
]

const steps = [
  {title: 'Регистрация', text: 'Создайте компанию и пригласите команду.'},
  {title: 'Импорт', text: 'Загрузите клиентов из Excel или добавляйте вручную.'},
  {title: 'Работа', text: 'Ведите заявки, платежи и документы ежедневно.'},
]

if (import.meta?.env?.DEV || process.env.NODE_ENV === 'development') {
  window.addEventListener('error', (e) => {
    if (e?.message && e.message.includes('ResizeObserver loop')) {
      e.stopImmediatePropagation();
    }
  }, true);

  window.addEventListener('unhandledrejection', (e) => {
    const msg = String(e?.reason?.message || e?.reason || '');
    if (msg.includes('ResizeObserver loop')) {
      e.preventDefault();
    }
  });
}
</script>

<style scoped>
.bg-page {
  background-color: var(--color-baby-powder);
  position: relative;
  overflow: auto;
}

:root {
  scrollbar-gutter: stable both-edges;
}

:deep(.v-card), .form-box {
  contain: layout paint;
}

.decoration-circle {
  position: absolute;
  border-radius: 50%;
  background: linear-gradient(135deg, var(--color-pear) 0%, rgba(206, 219, 149, 0.3) 100%);
  opacity: 0.1;
  z-index: 0;
}

.circle-1 {
  width: 300px;
  height: 300px;
  top: -150px;
  right: -150px;
}

.circle-2 {
  width: 200px;
  height: 200px;
  bottom: 100px;
  left: -100px;
}

.decoration-blob {
  position: absolute;
  border-radius: 50%;
  background: linear-gradient(135deg, var(--brand-primary) 0%, rgba(139, 146, 109, 0.2) 100%);
  opacity: 0.05;
  z-index: 0;
}

.blob-1 {
  width: 400px;
  height: 400px;
  top: 50%;
  right: 10%;
  transform: translateY(-50%);
}

.hero-section {
  position: relative;
  z-index: 1;
}

.badge {
  display: inline-block;
  background: rgba(139, 146, 109, 0.1);
  padding: 8px 16px;
  border-radius: 20px;
  border: 1px solid rgba(139, 146, 109, 0.2);
}

.badge-text {
  color: var(--brand-primary);
  font-weight: 500;
  font-size: 0.875rem;
}

.gradient-text {
  background: linear-gradient(135deg, var(--brand-ink) 0%, #2D3748 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.highlight-text {
  color: var(--brand-primary);
  font-weight: 500;
}

.trust-badge {
  display: flex;
  align-items: center;
  color: var(--color-gray);
  font-size: 0.875rem;
}

.hero-visual {
  position: relative;
  display: flex;
  justify-content: center;
  align-items: center;
}

.hero-image {
  border-radius: 24px;
  transform: translateY(0);
  transition: all 0.5s ease;
  filter: drop-shadow(0 20px 40px rgba(0, 0, 0, 0.1));
  position: relative;
  z-index: 2;
}

.hero-image:hover {
  transform: translateY(-10px) scale(1.02);
  filter: drop-shadow(0 30px 60px rgba(0, 0, 0, 0.15));
}

.floating-card {
  position: absolute;
  background: var(--brand-paper);
  padding: 12px 16px;
  border-radius: 16px;
  display: flex;
  align-items: center;
  gap: 8px;
  font-weight: 500;
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.1);
  border: 1px solid var(--color-silver);
  z-index: 3;
  animation: float 3s ease-in-out infinite;
}

.card-1 {
  top: 20%;
  left: -20px;
  animation-delay: 0s;
}

.card-2 {
  top: 50%;
  right: -30px;
  animation-delay: 1s;
}

.card-3 {
  bottom: 20%;
  left: -10px;
  animation-delay: 2s;
}

@keyframes float {
  0%, 100% {
    transform: translateY(0);
  }
  50% {
    transform: translateY(-10px);
  }
}

.stats-section {
  position: relative;
  z-index: 1;
}

.stats-card {
  transition: all 0.4s ease;
  border-radius: 20px;
}

.stats-card:hover {
  transform: translateY(-8px);
  background: var(--brand-paper) !important;
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.08);
}

.stat-icon-wrapper {
  display: inline-flex;
  padding: 16px;
  background: linear-gradient(135deg, rgba(139, 146, 109, 0.1) 0%, rgba(206, 219, 149, 0.05) 100%);
  border-radius: 16px;
}

.gradient-number {
  background: linear-gradient(135deg, var(--brand-ink) 0%, var(--brand-primary) 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.section-header {
  text-align: center;
  position: relative;
  z-index: 1;
}

.section-badge {
  display: inline-block;
  background: rgba(139, 146, 109, 0.1);
  color: var(--brand-primary);
  padding: 6px 16px;
  border-radius: 16px;
  font-weight: 500;
  font-size: 0.875rem;
  margin-bottom: 16px;
}

.feature-card {
  border-radius: 20px;
  transition: all 0.4s ease;
  position: relative;
  overflow: hidden;
  border: 1px solid transparent;
  background: linear-gradient(135deg, var(--brand-paper) 0%, rgba(255, 255, 255, 0.8) 100%);
}

.feature-card:hover {
  transform: translateY(-8px);
  border-color: var(--color-pear);
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.08);
}

.feature-icon-wrapper {
  display: inline-flex;
  padding: 12px;
  background: linear-gradient(135deg, rgba(139, 146, 109, 0.1) 0%, rgba(206, 219, 149, 0.05) 100%);
  border-radius: 12px;
}

.feature-hover-line {
  position: absolute;
  bottom: 0;
  left: 0;
  width: 0;
  height: 3px;
  background: linear-gradient(90deg, var(--color-pear), var(--brand-primary));
  transition: width 0.4s ease;
}

.feature-card:hover .feature-hover-line {
  width: 100%;
}

.steps-section {
  position: relative;
  z-index: 1;
}

.step-card {
  border-radius: 20px;
  transition: all 0.4s ease;
  background: linear-gradient(135deg, var(--brand-paper) 0%, rgba(255, 255, 255, 0.9) 100%);
}

.step-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 15px 30px rgba(0, 0, 0, 0.08);
}

.step-indicator {
  display: flex;
  align-items: center;
  justify-content: center;
}

.step-number {
  width: 50px;
  height: 50px;
  background: linear-gradient(135deg, var(--color-pear) 0%, var(--brand-primary) 100%);
  color: var(--brand-paper);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: bold;
  font-size: 1.25rem;
  box-shadow: 0 8px 20px rgba(139, 146, 109, 0.3);
}

.cta-card {
  border-radius: 28px;
  background: linear-gradient(135deg, var(--brand-paper) 0%, rgba(255, 255, 255, 0.95) 100%) !important;
  box-shadow: 0 20px 50px rgba(0, 0, 0, 0.08) !important;
  border: 1px solid rgba(139, 146, 109, 0.1);
  position: relative;
  z-index: 1;
  overflow: hidden;
}

.cta-card::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 4px;
  background: linear-gradient(90deg, var(--color-pear), var(--brand-primary));
}

.btn-pear {
  background: linear-gradient(135deg, var(--color-pear) 0%, #d4e2a8 100%) !important;
  color: var(--brand-ink) !important;
  font-weight: 600;
  border-radius: 14px !important;
  transition: all 0.4s ease !important;
  box-shadow: 0 4px 15px rgba(206, 219, 149, 0.3) !important;
  border: 1px solid rgba(206, 219, 149, 0.3) !important;
}

.btn-pear:hover {
  transform: translateY(-3px);
  box-shadow: 0 8px 25px rgba(206, 219, 149, 0.5) !important;
}

.link-brand {
  color: var(--brand-primary) !important;
  font-weight: 500;
  transition: all 0.3s ease !important;
}

.link-brand:hover {
  color: var(--brand-ink) !important;
  transform: translateX(5px);
}

.text-ink {
  color: var(--brand-ink);
}

.text-muted {
  color: var(--color-gray);
}

.bg-paper {
  background-color: var(--brand-paper);
}

@media (max-width: 960px) {
  .py-16 { padding-top: 32px !important; padding-bottom: 32px !important; }
  .hero-section { text-align: center; }
  .hero-content .text-h3 { font-size: 28px; line-height: 1.2; }
  .hero-content .text-h6 { font-size: 16px; }
  .hero-visual { margin-top: 16px; }
  .hero-image { max-width: 420px; width: 100%; border-radius: 20px; }
  .floating-card { display: none; }
  .stats-card, .feature-card, .step-card { border-radius: 16px; }
  .feature-card, .step-card { padding: 20px !important; }
  .step-number { width: 44px; height: 44px; font-size: 1.1rem; box-shadow: 0 6px 16px rgba(139,146,109,.25); }
  .section-header .text-h4 { font-size: 24px; }
  .cta-card { padding: 24px !important; border-radius: 22px; }
}

@media (max-width: 600px) {
  .decoration-circle, .decoration-blob { display: none; }
  .v-container { padding-left: 16px !important; padding-right: 16px !important; }
  .hero-content .text-h3 { font-size: 24px; }
  .hero-content .text-h6 { font-size: 15px; }
  .btn-pear { width: 100%; height: 48px !important; font-size: 15px; border-radius: 12px !important; }
  .trust-badge { margin-top: 8px; justify-content: center; }
  .hero-visual { margin-top: 12px; }
  .hero-image { max-width: 340px; filter: drop-shadow(0 14px 24px rgba(0,0,0,.10)); }
  .hero-image:hover { transform: translateY(-6px) scale(1.01); filter: drop-shadow(0 18px 34px rgba(0,0,0,.12)); }
  .feature-card, .step-card { margin-bottom: 12px; padding: 16px !important; }
  .feature-icon-wrapper { padding: 10px; border-radius: 10px; }
  .step-number { width: 40px; height: 40px; font-size: 1rem; }
  .stats-card .text-h3 { font-size: 24px; }
  .cta-card { padding: 20px !important; }
  .cta-card .d-flex { gap: 8px; }
}

@media (max-width: 420px) {
  .hero-content .text-h3 { font-size: 22px; }
  .hero-image { max-width: 300px; }
  .section-badge { font-size: 12px; padding: 5px 12px; }
  .gradient-number { -webkit-text-fill-color: currentColor; background: none; }
}

@media (prefers-reduced-motion: reduce) {
  .hero-image,
  .stats-card,
  .feature-card,
  .step-card,
  .floating-card,
  .btn-pear { transition: none !important; animation: none !important; }
}
</style>