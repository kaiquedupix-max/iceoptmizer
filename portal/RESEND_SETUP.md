# Configuração do Resend

1. Crie uma conta no Resend e adicione o domínio `iceotimizacoes.store`.
2. Copie exatamente os registros DNS mostrados pelo Resend para o painel DNS da Hostinger e aguarde o domínio aparecer como **Verified**.
3. Crie uma API Key com permissão de envio.
4. No Railway, adicione:

```text
RESEND_API_KEY=re_...
EMAIL_FROM=Ice Optimizer <contato@iceotimizacoes.store>
EMAIL_REPLY_TO=suporte@iceotimizacoes.store
ADMIN_EMAIL=email-que-recebera-alertas@exemplo.com
```

O endereço de `EMAIL_FROM` precisa usar o domínio verificado. `EMAIL_REPLY_TO` recebe respostas dos clientes e `ADMIN_EMAIL` recebe avisos de novos saques.

## Mensagens automáticas

- conta criada;
- solicitação e confirmação de troca de senha;
- pagamento em processamento, aprovado, recusado, cancelado ou devolvido;
- assinatura liberada;
- convite do afiliado e nova comissão;
- saque solicitado, aprovado, pago ou rejeitado.

Cada mensagem usa uma chave de idempotência para reduzir envios duplicados durante repetições de webhook.
