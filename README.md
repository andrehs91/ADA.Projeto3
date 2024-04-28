# ADA Tech - Módulo 3 (Conteinerização)

**Observação:** A lógica de negócio e a estrutura básica deste projeto foi aproveitada do projeto entregue no módulo 2.

### Estrutura do Projeto
A solução consiste em:
- Um **producer** (ASP.NET Core Web API) contendo uma interface Swagger para envio das transações para processamento e consulta aos relatórios;
- Um **broker** (RabbitMQ) para comunicação assíncrona entre os serviços;
- Um **consumer** (Worker Service) para processamento das transações;
- Um **cache** (Redis) para melhora no desempenho do processamento das transações;
- Um **sistema de armazenamento de objetos** (MinIO) para armazenamento dos relatórios gerados.
- Todos estes itens serão executados em um cluster Kubernetes.

### Regras de Negócio
&nbsp; &nbsp; O **producer** verifica apenas se os dados enviados estão de acordo com as restrições do objeto TransacaoDTO.\
&nbsp; &nbsp; Neste cenário, o **producer** envia a mensagem para uma *exchange* do tipo Fanout, que as distribui entre filas para efetivação da transação e verificação de fraudes.\
&nbsp; &nbsp; O **consumer** implementado neste projeto trata apenas da verificação de fraudes. Utilizando o cache da última transação válida, ele verifica a velocidade de deslocamento do cliente considerando as coordenadas geográficas destas transações.\
&nbsp; &nbsp; Caso uma nova transação seja efetuada no mesmo canal da anterior (Agência, Terminal de Auto Atendimento ou Internet Banking) e a velocidade de deslocamento calculada entre as duas localidades seja superior à 60 Km/h (valor arbitrário) o sistema identificará esta transação como fraudulenta e a incluirá em um conjunto armazenado em cache.\
&nbsp; &nbsp; A consulta aos relatórios deve ser feita no **producer**. As transações fraudulentas permanecerão em cache até que o relatório seja gerado. Quando ele for gerado, um arquivo será criado no MinIO e seu link será fornecido. A lista de links gerados por conta será armazenada em cache e também poderá ser consultada.

### Como Executar este Projeto Localmente

**Obs.:** Este projeto foi construído utilizando o Docker Desktop para rodar um *Kubernetes single-node cluster*. Caso esteja utilizando uma abordagem diferente, alterações poderão ser necessárias.

1. (Opcional) Caso teu cluster não possua um Ingress configurado, execute este comando: ` kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.1.0/deploy/static/provider/cloud/deploy.yaml `;
2. crie um servidor de métricas: ` kubectl apply -f 01.metrics-server.yaml `;
3. Crie o ConfigMap e as Secrets: ` kubectl apply -f 02.environment.yaml `;
4. Crie os PersistentVolumeClaim: ` kubectl apply -f 03.volumes.yaml `;
5. Execute o arquivo responsável por criar a solução: ` 04.solution.yaml `;
6. Acesse a interface do [Swagger](http://localhost/swagger/index.html) e envie algumas transações; [^1]
7. (Opcional) Acompanhe a fila de mensagens através do endereço [localhost:30001](http://localhost:30001/);
8. (Opcional) Acompanhe o processamento das mensagens através dos logs do consumer (os cálculos realizados e o resultado da validação são enviados para o log): ` kubectl -n projeto3 logs consumer-000000000-00000 --all-containers `; [^2]
9. Consulte e/ou liste os relatórios de uma conta;
10. (Opcional) Verifique os arquivos gerados no MinIO através do endereço [localhost:30002](http://localhost:30002/).

[^1]: Para que os testes possam ser realizados com facilidade, o campo Data é de preenchimento manual e permite inclusive datas passadas.
[^2]: Lembre-se de alterar o código do pod.
