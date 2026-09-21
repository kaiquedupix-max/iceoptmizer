# Configuração do Mercado Pago

1. Em **Suas integrações**, crie uma aplicação para pagamentos online.
2. Copie a Public Key para `MP_PUBLIC_KEY` e o Access Token para `MP_ACCESS_TOKEN` no Railway.
3. Em Webhooks, use `https://iceotimizacoes.store/api/webhooks/mercadopago`, habilite **Pagamentos** e copie a assinatura secreta para `MP_WEBHOOK_SECRET`.
4. Defina `PUBLIC_URL=https://iceotimizacoes.store` e mantenha `AFFILIATE_HOLD_DAYS=14`.
5. Cadastre uma chave Pix na conta Mercado Pago para receber pagamentos Pix.

O checkout usa o Payment Brick oficial com Pix e cartão. O servidor define os preços R$ 29,90 e R$ 100,00, consulta cada pagamento no Mercado Pago e ativa a conta somente quando o status é `approved`.

## Parceiros

O administrador cria o parceiro no painel e envia o acesso individual. O parceiro recebe um link `/r/nome-do-canal`, cadastra a chave Pix e acompanha a carteira. Cada venda aprovada gera 30% de comissão. O saldo fica em validação por 14 dias e depois pode ser solicitado no painel. O administrador realiza o Pix e marca o saque como pago.