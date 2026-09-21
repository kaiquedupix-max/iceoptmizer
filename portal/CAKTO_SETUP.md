# Configuração da Cakto

## Produtos e comissão

1. Crie na Cakto a oferta mensal de R$ 29,90 e a oferta permanente de R$ 100,00.
2. Ative o programa de afiliados dos dois produtos.
3. Defina a comissão dos divulgadores em **30%** na Cakto. O split e o pagamento da comissão são responsabilidade da Cakto.
4. Copie os IDs das ofertas e as credenciais da API de pagamentos para as variáveis do Railway.

## Webhook

Cadastre este endereço na Cakto:

`https://ice-optimizer-web-production.up.railway.app/api/webhooks/cakto`

Use o mesmo segredo em `CAKTO_WEBHOOK_SECRET` e no campo de segredo do webhook. Ative pelo menos estes eventos:

- `purchase_approved`
- `refund`
- `chargeback`
- `subscription_canceled`

## Variáveis do Railway

- `PUBLIC_URL`: endereço público do site, sem barra no final.
- `CAKTO_WEBHOOK_SECRET`: segredo exclusivo usado para validar notificações.
- `CAKTO_INVITE_URL`: convite do programa de afiliados na Cakto.
- `CAKTO_OFFER_D30`: ID da oferta mensal.
- `CAKTO_OFFER_PERMANENT`: ID da oferta permanente.
- `CAKTO_API_CLIENT_ID`: Client ID da API pública.
- `CAKTO_API_CLIENT_SECRET`: Client Secret da API pública.

## Fluxo do divulgador

1. No painel administrativo, abra **Parceiros** e gere o acesso do YouTuber.
2. Envie o endereço individual exibido uma única vez.
3. O parceiro abre o convite, aceita a afiliação na Cakto e informa no painel o e-mail e o short ID de afiliado criado pela plataforma.
4. O portal gera o link público no formato `/r/nome-do-canal`.
5. Cada venda aprovada é atribuída pelo e-mail de afiliado recebido no webhook. O painel mostra vendas, faturamento e o valor de comissão confirmado pela Cakto.

O comprador precisa usar no checkout o mesmo e-mail cadastrado no site. Assim que o webhook `purchase_approved` chegar, a conta é ativada sem intervenção manual.
