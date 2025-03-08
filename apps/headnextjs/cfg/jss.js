const config = {
    sitecoreApiKey: process.env.SITECORE_API_KEY ?? 'missing',
    sitecoreApiHost: process.env.SITECORE_API_HOST ?? '',
    sitecoreEdgeUrl: process.env.SITECORE_EDGE_URL ?? '',
    sitecoreEdgeContextId: process.env.SITECORE_EDGE_CONTEXT ?? '',
    sitecoreSiteName: process.env.SITECORE_SITE_NAME ?? 'website1',
    publicUrl: process.env.PUBLIC_URL ?? 'http://localhost:5000/',
    graphQLEndpointPath: process.env.GRAPH_QL_ENDPOINT_PATH ?? '/sitecore/api/graph/edge',
    defaultLanguage: process.env.DEFAULT_LANGUAGE ?? 'en',
    locales: ['en'],
    graphQLEndpoint: process.env.GRAPH_QL_ENDPOINT ?? '',
    searchCustomerKey: process.env.SEARCH_CUSTOMERKEY,
    searchApiKey: process.env.SEARCH_APIKEY,
    searchApiHost: process.env.SEARCH_APIHOST,
    searchEnv: process.env.SEARCH_ENV ?? 'prodEu',
  };
  
  module.exports = config;
  