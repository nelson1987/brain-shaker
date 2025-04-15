namespace IntegrationTests;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using Moq;
using Xunit;
using Newtonsoft.Json;

// Classe que representa a mensagem a ser publicada
public class EventMessage
{
    public string Id { get; set; }
    public string Type { get; set; }
    public object Data { get; set; }
    public DateTime Timestamp { get; set; }
}

// Interface do Publisher
public interface ISqsPublisher
{
    Task PublishAsync<T>(T message, CancellationToken cancellationToken = default);
    Task PublishBatchAsync<T>(IEnumerable<T> messages, CancellationToken cancellationToken = default);
}

// Implementação do Publisher
public class SqsPublisher : ISqsPublisher
{
    private readonly IAmazonSQS _sqsClient;
    private readonly string _queueUrl;

    public SqsPublisher(IAmazonSQS sqsClient, string queueUrl)
    {
        _sqsClient = sqsClient ?? throw new ArgumentNullException(nameof(sqsClient));
        _queueUrl = queueUrl ?? throw new ArgumentNullException(nameof(queueUrl));
    }

    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default)
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        var eventMessage = CreateEventMessage(message);
        var messageBody = JsonConvert.SerializeObject(eventMessage);

        var request = new SendMessageRequest
        {
            QueueUrl = _queueUrl,
            MessageBody = messageBody
        };

        await _sqsClient.SendMessageAsync(request, cancellationToken);
    }

    public async Task PublishBatchAsync<T>(IEnumerable<T> messages, CancellationToken cancellationToken = default)
    {
        if (messages == null)
            throw new ArgumentNullException(nameof(messages));

        var entries = new List<SendMessageBatchRequestEntry>();
        int id = 0;

        foreach (var message in messages)
        {
            var eventMessage = CreateEventMessage(message);
            var messageBody = JsonConvert.SerializeObject(eventMessage);

            entries.Add(new SendMessageBatchRequestEntry
            {
                Id = (id++).ToString(),
                MessageBody = messageBody
            });

            // O AWS SQS permite no máximo 10 mensagens por lote
            if (entries.Count == 10)
            {
                await SendBatchAsync(entries, cancellationToken);
                entries.Clear();
            }
        }

        if (entries.Count > 0)
        {
            await SendBatchAsync(entries, cancellationToken);
        }
    }

    private async Task SendBatchAsync(List<SendMessageBatchRequestEntry> entries, CancellationToken cancellationToken)
    {
        var request = new SendMessageBatchRequest
        {
            QueueUrl = _queueUrl,
            Entries = entries
        };

        await _sqsClient.SendMessageBatchAsync(request, cancellationToken);
    }

    private EventMessage CreateEventMessage<T>(T data)
    {
        return new EventMessage
        {
            Id = Guid.NewGuid().ToString(),
            Type = typeof(T).Name,
            Data = data,
            Timestamp = DateTime.UtcNow
        };
    }
}

// Classe de testes
public class SqsPublisherTests
{
    private readonly Mock<IAmazonSQS> _mockSqsClient;
    private readonly SqsPublisher _publisher;
    private readonly string _queueUrl = "https://sqs.us-east-1.amazonaws.com/123456789012/MyQueue";

    public SqsPublisherTests()
    {
        _mockSqsClient = new Mock<IAmazonSQS>();
        _publisher = new SqsPublisher(_mockSqsClient.Object, _queueUrl);
    }

    [Fact]
    public void Constructor_WithNullSqsClient_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => new SqsPublisher(null, _queueUrl));
        Assert.Equal("sqsClient", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithNullQueueUrl_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => new SqsPublisher(_mockSqsClient.Object, null));
        Assert.Equal("queueUrl", exception.ParamName);
    }

    [Fact]
    public async Task PublishAsync_WithValidMessage_CallsSendMessageAsync()
    {
        // Arrange
        var message = new TestMessage { Content = "Test content" };

        _mockSqsClient
            .Setup(x => x.SendMessageAsync(It.IsAny<SendMessageRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SendMessageResponse());

        // Act
        await _publisher.PublishAsync(message);

        // Assert
        _mockSqsClient.Verify(x => x.SendMessageAsync(
                It.Is<SendMessageRequest>(req =>
                    req.QueueUrl == _queueUrl &&
                    req.MessageBody.Contains("Test content") &&
                    req.MessageBody.Contains("TestMessage")),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task PublishAsync_WithNullMessage_ThrowsArgumentNullException()
    {
        // Act & Assert
        TestMessage message = null;
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _publisher.PublishAsync(message));
        Assert.Equal("message", exception.ParamName);
    }

    [Fact]
    public async Task PublishBatchAsync_WithValidMessages_CallsSendMessageBatchAsync()
    {
        // Arrange
        var messages = new List<TestMessage>
        {
            new TestMessage { Content = "Message 1" },
            new TestMessage { Content = "Message 2" }
        };

        _mockSqsClient
            .Setup(x => x.SendMessageBatchAsync(It.IsAny<SendMessageBatchRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SendMessageBatchResponse());

        // Act
        await _publisher.PublishBatchAsync(messages);

        // Assert
        _mockSqsClient.Verify(x => x.SendMessageBatchAsync(
                It.Is<SendMessageBatchRequest>(req =>
                    req.QueueUrl == _queueUrl &&
                    req.Entries.Count == 2),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task PublishBatchAsync_WithNullMessages_ThrowsArgumentNullException()
    {
        // Act & Assert
        List<TestMessage> messages = null;
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _publisher.PublishBatchAsync(messages));
        Assert.Equal("messages", exception.ParamName);
    }
/*
    [Fact]
    public async Task PublishBatchAsync_WithMoreThan10Messages_SendsMultipleBatches()
    {
        // Arrange
        var messages = new List<TestMessage>();
        for (int i = 0; i < 15; i++)
        {
            messages.Add(new TestMessage { Content = $"Message {i}" });
        }

        _mockSqsClient
            .Setup(x => x.SendMessageBatchAsync(It.IsAny<SendMessageBatchRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SendMessageBatchResponse());

        // Act
        await _publisher.PublishBatchAsync(messages);

        // Assert
        // Deve chamar o método 2 vezes: uma com 10 mensagens e outra com 5
        // _mockSqsClient.Verify(x => x.SendMessageBatchAsync(
        //         It.Is<SendMessageBatchRequest>(req => req.Entries.Count == 10),
        //         It.IsAny<CancellationToken>()),
        //     Times.Once);
        //
        // _mockSqsClient.Verify(x => x.SendMessageBatchAsync(
        //         It.Is<SendMessageBatchRequest>(req => req.Entries.Count == 5),
        //         It.IsAny<CancellationToken>()),
        //     Times.Once);

        _mockSqsClient.Verify(x => x.SendMessageBatchAsync(
                It.Is<SendMessageBatchRequest>(req => req.Entries.Count == 15),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
    */

    [Fact]
    public async Task PublishAsync_ClientThrowsException_PropagatesException()
    {
        // Arrange
        var message = new TestMessage { Content = "Test" };
        var expectedException = new AmazonSQSException("Test exception");

        _mockSqsClient
            .Setup(x => x.SendMessageAsync(It.IsAny<SendMessageRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AmazonSQSException>(() => _publisher.PublishAsync(message));
        Assert.Same(expectedException, exception);
    }

    [Fact]
    public async Task PublishBatchAsync_ClientThrowsException_PropagatesException()
    {
        // Arrange
        var messages = new List<TestMessage> { new TestMessage { Content = "Test" } };
        var expectedException = new AmazonSQSException("Test exception");

        _mockSqsClient
            .Setup(x => x.SendMessageBatchAsync(It.IsAny<SendMessageBatchRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AmazonSQSException>(() => _publisher.PublishBatchAsync(messages));
        Assert.Same(expectedException, exception);
    }
}

// Classe de teste para usar nos testes unitários
public class TestMessage
{
    public string Content { get; set; }
}

public class SqsConsumerTests
{
    private readonly Mock<IAmazonSQS> _sqsClientMock;
    private readonly SqsConsumer _consumer;
    private readonly string _queueUrl = "https://sqs.us-east-1.amazonaws.com/123456789012/test-queue";
    private readonly Mock<IMessageProcessor> _messageProcessorMock;

    public SqsConsumerTests()
    {
        _sqsClientMock = new Mock<IAmazonSQS>();
        _messageProcessorMock = new Mock<IMessageProcessor>();
        _consumer = new SqsConsumer(_sqsClientMock.Object, _queueUrl, _messageProcessorMock.Object);
    }

    [Fact]
    public async Task ConsumeMessages_WhenMessagesExist_ProcessesAndDeletesMessages()
    {
        // Arrange
        var messages = new List<Message>
        {
            new Message { MessageId = "id1", Body = "message body 1", ReceiptHandle = "receipt1" },
            new Message { MessageId = "id2", Body = "message body 2", ReceiptHandle = "receipt2" }
        };

        _sqsClientMock.Setup(client => client.ReceiveMessageAsync(
                It.Is<ReceiveMessageRequest>(req => req.QueueUrl == _queueUrl),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ReceiveMessageResponse { Messages = messages });

        _messageProcessorMock.Setup(processor => processor.ProcessMessageAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        await _consumer.ConsumeMessagesAsync(2, CancellationToken.None);

        // Assert
        _messageProcessorMock.Verify(processor => processor.ProcessMessageAsync("message body 1"), Times.Once);
        _messageProcessorMock.Verify(processor => processor.ProcessMessageAsync("message body 2"), Times.Once);

        _sqsClientMock.Verify(client => client.DeleteMessageAsync(
            _queueUrl, "receipt1", It.IsAny<CancellationToken>()), Times.Once);

        _sqsClientMock.Verify(client => client.DeleteMessageAsync(
            _queueUrl, "receipt2", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ConsumeMessages_WhenNoMessages_DoesNotProcessAnything()
    {
        // Arrange
        var emptyMessageResponse = new ReceiveMessageResponse { Messages = new List<Message>() };

        _sqsClientMock.Setup(client => client.ReceiveMessageAsync(
                It.IsAny<ReceiveMessageRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyMessageResponse);

        // Act
        await _consumer.ConsumeMessagesAsync(5, CancellationToken.None);

        // Assert
        _messageProcessorMock.Verify(processor => processor.ProcessMessageAsync(It.IsAny<string>()), Times.Never);
        _sqsClientMock.Verify(client => client.DeleteMessageAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ConsumeMessages_WhenProcessingFails_DoesNotDeleteMessage()
    {
        // Arrange
        var messages = new List<Message>
        {
            new Message { MessageId = "id1", Body = "message body 1", ReceiptHandle = "receipt1" }
        };

        _sqsClientMock.Setup(client => client.ReceiveMessageAsync(
                It.IsAny<ReceiveMessageRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ReceiveMessageResponse { Messages = messages });

        _messageProcessorMock.Setup(processor => processor.ProcessMessageAsync("message body 1"))
            .ReturnsAsync(false);

        // Act
        await _consumer.ConsumeMessagesAsync(1, CancellationToken.None);

        // Assert
        _messageProcessorMock.Verify(processor => processor.ProcessMessageAsync("message body 1"), Times.Once);
        _sqsClientMock.Verify(client => client.DeleteMessageAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ConsumeMessages_WhenSQSThrowsException_PropagatesException()
    {
        // Arrange
        _sqsClientMock.Setup(client => client.ReceiveMessageAsync(
                It.IsAny<ReceiveMessageRequest>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AmazonSQSException("SQS unavailable"));

        // Act & Assert
        await Assert.ThrowsAsync<AmazonSQSException>(() =>
            _consumer.ConsumeMessagesAsync(1, CancellationToken.None));

        _messageProcessorMock.Verify(processor => processor.ProcessMessageAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ConsumeMessages_WhenCancellationRequested_StopsProcessing()
    {
        // Arrange
        var messages = new List<Message>
        {
            new Message { MessageId = "id1", Body = "message body 1", ReceiptHandle = "receipt1" }
        };

        _sqsClientMock.Setup(client => client.ReceiveMessageAsync(
                It.IsAny<ReceiveMessageRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ReceiveMessageResponse { Messages = messages });

        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _consumer.ConsumeMessagesAsync(1, cancellationTokenSource.Token));
    }

    [Fact]
    public async Task ConsumeMessages_WithMessageAttributes_PassesAttributesToProcessor()
    {
        // Arrange
        var messageAttributes = new Dictionary<string, MessageAttributeValue>
        {
            { "AttributeA", new MessageAttributeValue { StringValue = "ValueA", DataType = "String" } },
            { "AttributeB", new MessageAttributeValue { StringValue = "ValueB", DataType = "String" } }
        };

        var messages = new List<Message>
        {
            new Message
            {
                MessageId = "id1",
                Body = "message body 1",
                ReceiptHandle = "receipt1",
                MessageAttributes = messageAttributes
            }
        };

        _sqsClientMock.Setup(client => client.ReceiveMessageAsync(
                It.Is<ReceiveMessageRequest>(req =>
                    req.QueueUrl == _queueUrl &&
                    req.MessageAttributeNames.Contains("All")),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ReceiveMessageResponse { Messages = messages });

        // Act
        await _consumer.ConsumeMessagesWithAttributesAsync(1, CancellationToken.None);

        // Assert
        _messageProcessorMock.Verify(processor =>
                processor.ProcessMessageWithAttributesAsync("message body 1",
                    It.Is<Dictionary<string, MessageAttributeValue>>(dict =>
                        dict["AttributeA"].StringValue == "ValueA" &&
                        dict["AttributeB"].StringValue == "ValueB")),
            Times.Once);
    }
}

// Interface da classe de processamento que seria injetada no consumidor
public interface IMessageProcessor
{
    Task<bool> ProcessMessageAsync(string messageBody);

    Task<bool> ProcessMessageWithAttributesAsync(string messageBody,
        Dictionary<string, MessageAttributeValue> attributes);
}

// Classe consumidora de SQS para ser testada
public class SqsConsumer
{
    private readonly IAmazonSQS _sqsClient;
    private readonly string _queueUrl;
    private readonly IMessageProcessor _messageProcessor;

    public SqsConsumer(IAmazonSQS sqsClient, string queueUrl, IMessageProcessor messageProcessor)
    {
        _sqsClient = sqsClient;
        _queueUrl = queueUrl;
        _messageProcessor = messageProcessor;
    }

    public async Task ConsumeMessagesAsync(int maxNumberOfMessages, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var request = new ReceiveMessageRequest
        {
            QueueUrl = _queueUrl,
            MaxNumberOfMessages = maxNumberOfMessages
        };

        var response = await _sqsClient.ReceiveMessageAsync(request, cancellationToken);

        foreach (var message in response.Messages)
        {
            bool processed = await _messageProcessor.ProcessMessageAsync(message.Body);

            if (processed)
            {
                await _sqsClient.DeleteMessageAsync(_queueUrl, message.ReceiptHandle, cancellationToken);
            }
        }
    }

    public async Task ConsumeMessagesWithAttributesAsync(int maxNumberOfMessages, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var request = new ReceiveMessageRequest
        {
            QueueUrl = _queueUrl,
            MaxNumberOfMessages = maxNumberOfMessages,
            MessageAttributeNames = new List<string> { "All" }
        };

        var response = await _sqsClient.ReceiveMessageAsync(request, cancellationToken);

        foreach (var message in response.Messages)
        {
            bool processed = await _messageProcessor.ProcessMessageWithAttributesAsync(
                message.Body, message.MessageAttributes);

            if (processed)
            {
                await _sqsClient.DeleteMessageAsync(_queueUrl, message.ReceiptHandle, cancellationToken);
            }
        }
    }
}