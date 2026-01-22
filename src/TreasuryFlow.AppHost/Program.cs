var builder = DistributedApplication.CreateBuilder(args);

var username = builder.AddParameter("username", value: "admin", secret: true);
var password = builder.AddParameter("password", value: "541981", secret: true);

var sql = builder
    .AddSqlServer("sqlserver")
    .WithEnvironment("ACCEPT_EULA", "Y")
    .WithEnvironment("SA_PASSWORD", "1q2w3e4r@#$")
    .AddDatabase("treasury-flow");

var rabbitmq = builder.AddRabbitMQ("RabbitMq", userName: username, password: password, port: 5672)
    .WithManagementPlugin(15672);

builder.AddProject<Projects.TreasuryFlow_Api>("treasuryflow-api")
    .WithReference(sql)
    .WaitFor(sql)
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq);

builder.Build().Run();
