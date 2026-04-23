<template>
  <v-footer app height="auto" class="app-footer">
    <v-container class="footer-container">
      <div class="footer-left">
        <div class="brand-info">
          <span>© SapARide, 2025</span>
        </div>
      </div>

      <v-card variant="flat" class="footer-middle pill contact-card">
        <div class="contact-icon">
          <v-icon icon="mdi-phone" size="18" class="icon"/>
        </div>
        <div class="contact-content">
          <div class="contact-number">+7 (777) 777-77-77</div>
          <div class="muted">
            <v-icon icon="mdi-email" size="14" class="email-icon"/>
            saparide@gmail.com
          </div>
        </div>
      </v-card>

      <v-card variant="flat" class="footer-right pill cta-card">
        <div class="cta-content">
          <div class="cta-icon">
            <v-icon icon="mdi-help-circle" size="20" class="icon"/>
          </div>
          <div class="cta-text">
            <div>Есть вопросы</div>
            <div>или предложения?</div>
          </div>
        </div>
        <v-btn
            class="cta-btn"
            variant="flat"
            :ripple="false"
            @click="contact"
        >
          <v-icon icon="mdi-message-text" size="16" class="btn-icon"/>
          Напишите нам
        </v-btn>
      </v-card>
    </v-container>

    <div class="footer-accent"></div>
  </v-footer>
</template>

<script setup>
function contact () {
  window.location.href = 'mailto:info@example.com'
}
</script>

<style scoped>
.app-footer{
  background: var(--color-white);
  border-top: 1px solid rgba(0,0,0,.06);
  padding: 12px 0;
  padding-inline-start: var(--v-layout-left);
  margin-inline-start: 0 !important;
  position: relative;
  z-index: 4000;
  inset-inline-start: 0 !important;
  width: 100% !important;
}

.app-footer::before{
  content: "";
  position: absolute;
  inset: 0;
  inset-inline-start: calc(-1 * var(--v-layout-left));
  background: var(--color-white);
  border-top: 1px solid rgba(0,0,0,.06);
  z-index: -1;
}

.footer-container{
  margin: 0 auto;
  padding: 0 16px;
  display: grid;
  grid-template-columns: 1fr auto auto;
  gap: 16px;
  align-items: center;
  max-width: 1430px !important;
}

.footer-left {
  color: black;
}

.brand-info {
  display: flex;
  align-items: center;
  gap: 8px;
  font-weight: 500;
}

.pill{
  background: var(--color-baby-powder, #F2F3ED);
  border-radius: var(--radius-2xl, 16px);
  padding: 10px 14px;
  display: flex;
  align-items: center;
  gap: 12px;
  height: 80px;
  font-size: 16px;
  border: 1px solid transparent;
  transition: all 0.3s ease;
}

.contact-card {
  min-width: 180px;
  background: linear-gradient(135deg, var(--color-baby-powder) 0%, rgba(242, 243, 237, 0.8) 100%);
}

.contact-card:hover {
  border-color: rgba(139, 146, 109, 0.2);
  background: linear-gradient(135deg, var(--color-baby-powder) 0%, rgba(242, 243, 237, 0.8) 100%);
  transform: translateY(-1px);
}

.contact-icon {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 36px;
  height: 36px;
  background: rgba(139, 146, 109, 0.1);
  border-radius: 10px;
  flex-shrink: 0;
}

.contact-content {
  text-align: center;
  line-height: 1.2;
  flex: 1;
}

.contact-number {
  font-weight: 600;
  color: var(--brand-ink);
  margin-bottom: 2px;
}

.muted{
  font-size: 12px;
  opacity: .7;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 4px;
}

.email-icon {
  opacity: 0.7;
}

.icon {
  color: var(--brand-primary);
  opacity: 0.9;
}

.footer-right{
  min-height: 48px;
  padding-right: 8px;
  transition: all 0.3s ease;
  background: linear-gradient(135deg, var(--color-baby-powder) 0%, rgba(242, 243, 237, 0.9) 100%);
}

.cta-card:hover {
  background: linear-gradient(135deg, var(--color-baby-powder) 0%, rgba(242, 243, 237, 0.9) 100%);
  border-color: rgba(139, 146, 109, 0.2);
  transform: translateY(-1px);
}

.cta-content {
  display: flex;
  align-items: center;
  gap: 10px;
  flex: 1;
}

.cta-icon {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 36px;
  height: 36px;
  background: rgba(139, 146, 109, 0.1);
  border-radius: 10px;
  flex-shrink: 0;
}

.cta-text{
  line-height: 1.2;
  font-size: 14px;
  color: var(--brand-ink);
}

.cta-btn{
  padding-inline: 18px;
  height: 36px;
  background: var(--color-pear) !important;
  color: black !important;
  text-transform: none;
  letter-spacing: 0;
  border-radius: 12px !important;
  font-weight: 600;
  transition: all 0.3s ease !important;
  box-shadow: 0 2px 8px rgba(206, 219, 149, 0.3) !important;
  border: 1px solid rgba(206, 219, 149, 0.3) !important;
}

.cta-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 15px rgba(206, 219, 149, 0.5) !important;
}

.btn-icon {
  margin-right: 6px;
}

.footer-accent {
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  height: 1px;
  background: linear-gradient(90deg,
  transparent 0%,
  var(--color-pear) 25%,
  var(--brand-primary) 50%,
  var(--color-pear) 75%,
  transparent 100%);
  opacity: 0.6;
}

@media (max-width: 960px){
  .footer-container{
    grid-template-columns: 1fr;
    justify-items: stretch;
    gap: 12px;
  }

  .footer-right{
    justify-content: space-between;
  }

  .brand-info {
    justify-content: center;
  }

  .contact-card,
  .cta-card {
    justify-content: center;
    text-align: center;
  }

  .cta-content {
    justify-content: center;
  }
}

@media (max-width: 600px) {
  .footer-container {
    padding: 0 12px;
  }

  .pill {
    padding: 8px 12px;
    height: 70px;
  }

  .cta-btn {
    padding-inline: 14px;
    font-size: 14px;
  }

  .btn-icon {
    margin-right: 4px;
  }
}

.cta-btn:focus-visible {
  outline: 2px solid var(--brand-primary);
  outline-offset: 2px;
}
</style>