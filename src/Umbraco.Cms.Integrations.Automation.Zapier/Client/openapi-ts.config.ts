export default {
    input: 'http://localhost:6583/umbraco/swagger/zapier-management/swagger.json',
    output: {
      lint: 'eslint',
      path: 'generated',
    },
    schemas: false,
    services: {
      asClass: true
    },
    types: {
      enums: 'typescript',
    }
  }