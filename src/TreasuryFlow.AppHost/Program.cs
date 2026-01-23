var builder = DistributedApplication.CreateBuilder(args);

var username = builder.AddParameter("username", value: "admin", secret: true);
var password = builder.AddParameter("password", value: "541981", secret: true);

//var sql = builder
//    .AddSqlServer("sqlserver")
//    .WithEnvironment("ACCEPT_EULA", "Y")
//    .WithEnvironment("SA_PASSWORD", "1q2w3e4r@#$")
//    .AddDatabase("TreasuryFlowDb");

//var sqlPassword = builder.AddParameter(
//    "sql-sa-password",
//    value: "1q2w3e4r@#$",
//    secret: true);

//var sql = builder
//    .AddSqlServer("sqlserver")
//    .WithEnvironment("ACCEPT_EULA", "Y")
//    .WithEnvironment("SA_PASSWORD", sqlPassword)
//    .AddDatabase("TreasuryFlowDb");

var rabbitmq = builder.AddRabbitMQ("RabbitMq", userName: username, password: password, port: 5672)
    .WithManagementPlugin(15672);

builder.AddProject<Projects.TreasuryFlow_Api>("treasuryflow-api")
    //.WithReference(sql)
    //.WaitFor(sql)
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq);

builder.AddProject<Projects.TreasuryFlow_Consumer>("treasuryflow-consumer")
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq);

builder.Build().Run();
