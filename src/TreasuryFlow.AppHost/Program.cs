var builder = DistributedApplication.CreateBuilder(args);

var username = builder.AddParameter("username", value: "admin", secret: true);
var password = builder.AddParameter("password", value: "541981", secret: true);

var sqlPass = ResourceBuilder.Create(new ParameterResource("sqlpass", x => "1q2w3e4r@#$", true), builder);

var sql = builder.AddSqlServer("sqlserver", password: sqlPass, port: 57084)
    .WithDataVolume("mssql")
    .AddDatabase("TreasuryFlowDb");

var rabbitmq = builder.AddRabbitMQ("RabbitMq", userName: username, password: password, port: 5672)
    .WithManagementPlugin(15672);

builder.AddProject<Projects.TreasuryFlow_Api>("treasuryflow-api")
    .WithReference(sql)
    .WaitFor(sql)
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq);

builder.AddProject<Projects.TreasuryFlow_Consumer>("treasuryflow-consumer")
    .WithReference(sql)
    .WaitFor(sql)
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq);

builder.Build().Run();

class ResourceBuilder
{
    public static IResourceBuilder<T> Create<T>(T resource, IDistributedApplicationBuilder distributedApplicationBuilder) where T : IResource
    {
        return new ResourceBuilder<T>(resource, distributedApplicationBuilder);
    }
}

class ResourceBuilder<T>(T resource, IDistributedApplicationBuilder distributedApplicationBuilder) : IResourceBuilder<T> where T : IResource
{
    public IDistributedApplicationBuilder ApplicationBuilder { get; } = distributedApplicationBuilder;

    public T Resource { get; } = resource;

    public IResourceBuilder<T> WithAnnotation<TAnnotation>(TAnnotation annotation, ResourceAnnotationMutationBehavior behavior = ResourceAnnotationMutationBehavior.Append) where TAnnotation : IResourceAnnotation
    {
        throw new NotImplementedException();
    }
}