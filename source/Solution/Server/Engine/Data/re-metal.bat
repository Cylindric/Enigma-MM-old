@ECHO OFF

:: Rebuild the Data Context
:: The source database file needs to be in the build output directory

sqlmetal ../../../../Build/data.sdf /dbml:EMMDataContext.dbml /pluralize /context:EMMDataContext