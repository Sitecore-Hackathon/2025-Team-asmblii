// import { OTLPTraceExporter } from '@opentelemetry/exporter-trace-otlp-http'
// import { Resource } from '@opentelemetry/resources'
import { NodeSDK } from '@opentelemetry/sdk-node'
// import { SimpleSpanProcessor } from '@opentelemetry/sdk-trace-node'
// import { ATTR_SERVICE_NAME } from '@opentelemetry/semantic-conventions'
// import { OTLPLogExporter } from "@opentelemetry/exporter-logs-otlp-http";
// import { ConsoleSpanExporter } from '@opentelemetry/sdk-trace-node';
// import { getNodeAutoInstrumentations } from '@opentelemetry/auto-instrumentations-node';
// import {
//   PeriodicExportingMetricReader,
//   ConsoleMetricExporter,
// } from '@opentelemetry/sdk-metrics';
// import { BatchLogRecordProcessor, SimpleLogRecordProcessor } from "@opentelemetry/sdk-logs";

import config from '@jssconfig';

const serviceName = 'nextjs-' + config.sitecoreSiteName;
// console.log(`Starting instrumentation with ${serviceName} to ${process.env.OTEL_EXPORTER_OTLP_TRACES_ENDPOINT}`);


  // const sdk = new NodeSDK({
  //   mergeResourceWithDefaults: false,
  //   //traceExporter: new ConsoleSpanExporter(),
  //   // metricReader: new PeriodicExportingMetricReader({
  //   //   exporter: new ConsoleMetricExporter(),
  //   // }),
  //   // logRecordProcessors: [new  SimpleLogRecordProcessor(new OTLPLogExporter())],
  //   //instrumentations: [getNodeAutoInstrumentations()],
  // });

  // sdk.start();

  // const sdk = new NodeSDK({
  //   resource: new Resource({
  //     [ATTR_SERVICE_NAME]: serviceName,
  //   }),
  //   spanProcessor: new SimpleSpanProcessor(new OTLPTraceExporter()),
  // })
  // sdk.start();
