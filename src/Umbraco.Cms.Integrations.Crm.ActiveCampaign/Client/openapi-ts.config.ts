export default {
    input: 'http://localhost:40714/umbraco/swagger/activecampaign-forms-management/swagger.json',
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
  