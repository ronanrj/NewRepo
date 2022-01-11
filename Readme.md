Objetivo

Dado que a execução dos scripts de criação ou atualização de regras de domínio é feito manualmente

Queremos através desta issue, revisar as possibilidades e propor uma solução para a criação de uma pipeline de execução de scripts e promoção entre ambientes, similar ao que temos para nossas aplicações

Desta forma garantindo qualidade e acertividade nas entregas mitigando erro humano.

link : https://jira.viavarejo.com.br/browse/COM-3089
Fluxo Esperado

[Jornada Customer Success > Pipeline para execução de scripts de banco no Atlas Online > 68747470733a2f2f6d656469612e67697068792e636f6d2f6d656469612f3130744c4f46584446446a67514d2f67697068792e676966.gif]


Aplicabilidade com mongosh  (tentativa de automação de script)
Requisitos:

    https://docs.mongodb.com/mongodb-shell/
    mongo

Execuções:

Utilizando o comando mongodb://127.0.0.1:27017/SAC_PlataformaComunicacao para conectar a base local.

Escrito o Script3.js no qual, passamos a operação a executar (Change ou Rollback)  faz a varredura do diretório com os scripts e retornar uma lista de resultados (scripts).

Exemplo:

[Jornada Customer Success > Pipeline para execução de scripts de banco no Atlas Online > image2022-1-7_15-31-4.png]

Abaixo segue exemplo de arquivo Change_CriarAplicabilidade-004.js que irá rodar ao retornar o resultado do Script3.js

[Jornada Customer Success > Pipeline para execução de scripts de banco no Atlas Online > image2022-1-7_15-35-53.png]

Ao retornar a lista com os scripts , é percorrido e rodado cada script, fazendo assim a modificação em banco de dados.

for(var i=0;i<caminhosScriptsList.length;i++) load(caminhosScriptsList[i])
Exemplo completo:

Ex: mongosh mongodb://127.0.0.1:27017/SAC_PlataformaComunicacao --eval="load('Script3.js');for(var i=0;i<caminhosScriptsList.length;i++) load(caminhosScriptsList[i]);"
Dúvidas encontradas para mongosh:

 - Foi encontrado problemas com o formato de _id , o mongo possui formato LUUID e não aceita o GUID , teria que criar uma function para fazer esta conversão e inserir no script.(DEBITO TECNICO).

- Foi encontrado problemas ao rodar os scripts da pasta, se um script desse erro , o processo parava  , assim, não conseguindo prosseguir , gerando inconsistência por não saber , quais scripts tinham rodado .

- Como seria versionado o script rodado, a ferramenta não possui hoje a opção.
Observações sobre:

Verificado com a equipe de Plataforma que a solução proposta com o mongosh não era robusta para ser utilizada pela tribo.

Então, os dev´s da Plataforma (VICTOR RIOS DE SOUZA , JHONATAN DE SOUZA TEIXEIRA) em reunião com dev´s backend da squad(comunicação) ,decidimos criar uma aplicação estilo migration em .NET para realizar esta spike.

Ficou definido que a equipe de Plataforma iria abrir uma Issue no Jira para realizar esta tarefa, e eu iria fazer uma poc em paralelo.


Aplicabilidade com Mongo Migration .NET
Requisitos:

    https://github.com/SRoddis/Mongo.Migration
    https://bitbucket.org/i_am_a_kernel/mongodbmigrations/src/master/ReleaseNotes.md

Execuções:

Criado uma solução WebCore 3.1

Conectado a base local mongodb://127.0.0.1:27017/SAC_PlataformaComunicacao para teste.


[Jornada Customer Success > Pipeline para execução de scripts de banco no Atlas Online > image2022-1-10_9-33-23.png]


Execução via arquivo BsonDocuments:

[Jornada Customer Success > Pipeline para execução de scripts de banco no Atlas Online > image2022-1-10_11-11-7.png]

Criado a inserção do script via Bson

    Este formato não requer o esquema do objeto , então , Os documentos bson podem ser criados através da leitura de um arquivo um json.(método lendo os arquivos e montando).
    Neste formato o versionamento é inserido em cada documento bson.

Execução via Objeto:

[Jornada Customer Success > Pipeline para execução de scripts de banco no Atlas Online > image2022-1-10_11-28-22.png]

[Jornada Customer Success > Pipeline para execução de scripts de banco no Atlas Online > image2022-1-10_11-38-7.png]

Criado a inserção do script via Objeto

    Neste formato, precisamos saber qual o esquema do documento, porém como o banco de dados Mongo é não relacional , perdemos funcionalidades, isso é:
    Precisamos modelar o objeto conforme o documento na colletion ,porém, como o mongo é não relacional e pode ser inserido qualquer propriedade, este modelo fica desatualizado quebrando o programa.
    Foi necessário implementar a interface IDocument para realizar o versionamento do documento.


Exemplo completo:



atualizando....
