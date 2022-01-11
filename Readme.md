Objetivo

Dado que a execução dos scripts de criação ou atualização de regras de domínio é feito manualmente

Queremos através desta issue, revisar as possibilidades e propor uma solução para a criação de uma pipeline de execução de scripts e promoção entre ambientes, similar ao que temos para nossas aplicações

Desta forma garantindo qualidade e acertividade nas entregas mitigando erro humano.


Aplicabilidade com Mongo Migration .NET
Requisitos:

    https://github.com/SRoddis/Mongo.Migration
    https://bitbucket.org/i_am_a_kernel/mongodbmigrations/src/master/ReleaseNotes.md

Execuções:

Criado uma solução WebCore 3.1

Criado a inserção do script via Bson

    Este formato não requer o esquema do objeto , então , Os documentos bson podem ser criados através da leitura de um arquivo um json.(método lendo os arquivos e montando).
    Neste formato o versionamento é inserido em cada documento bson.

Execução via Objeto:

Criado a inserção do script via Objeto

    Neste formato, precisamos saber qual o esquema do documento, porém como o banco de dados Mongo é não relacional , perdemos funcionalidades, isso é:
    Precisamos modelar o objeto conforme o documento na colletion ,porém, como o mongo é não relacional e pode ser inserido qualquer propriedade, este modelo fica desatualizado quebrando o programa.
    Foi necessário implementar a interface IDocument para realizar o versionamento do documento.



