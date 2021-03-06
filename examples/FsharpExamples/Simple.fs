﻿module Simple

// Learn more about F# at http://fsharp.org
open System
open Pulsar.Client.Api
open FSharp.Control.Tasks.V2.ContextInsensitive
open Pulsar.Client.Common
open System.Text

let runSimple () =

    let serviceUrl = "pulsar://my-pulsar-cluster:30002"
    let subscriptionName = "my-subscription"
    let topicName = sprintf "my-topic-%i" DateTime.Now.Ticks;

    let client =
        PulsarClientBuilder()
            .ServiceUrl(serviceUrl)
            .Build()

    task {

        let! producer =
            client.NewProducer()
                .Topic(topicName)
                .CreateAsync()

        let! consumer =
            client.NewConsumer()
                .Topic(topicName)
                .SubscriptionName(subscriptionName)
                .SubscriptionType(SubscriptionType.Exclusive)
                .SubscribeAsync()        
        
        let! messageId = producer.SendAsync(Encoding.UTF8.GetBytes("Sent from F# at " + DateTime.Now.ToString()))
        printfn "MessageId is: '%A'" messageId

        let! message = consumer.ReceiveAsync()
        printfn "Received: %A" (message.Data |> Encoding.UTF8.GetString)

        do! consumer.AcknowledgeAsync(message.MessageId)
    }
