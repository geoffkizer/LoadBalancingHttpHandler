using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace sampleapp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            HttpClient client = new HttpClient(
                new LoadBalancingHttpHandler(new MyWrapperHttpHandler(new SocketsHttpHandler()))
                {
                    LoadBalancingPolicy = new PowerOfTwoLoadBalancingPolicy(),
                    PassiveHealthPolicy = new SimplePassiveHealthPolicy()
                    {
                        ObservationPeriod = TimeSpan.FromMinutes(1),
                        FailureThreshold = 1,
                        DeactivationPeriod = TimeSpan.FromSeconds(5)
                    },
                    DestinationManager = new StaticDestinationManager()
                    {
                        new Destination { Uri = new Uri("http://microsoft.com/") },
                        new Destination { Uri = new Uri("http://google.com/") },
                        new Destination { Uri = new Uri("http://yahoo.com/") },
                        new Destination { Uri = new Uri("http://bing.com/") },
                        new Destination { Uri = new Uri("http://msn.com/") },
                    }
                });

            int request = 0;
            while (true)
            {
                using (HttpResponseMessage message = await client.GetAsync($"http://myservice/?request={request}"))
                {
                    //Console.WriteLine($"Response: Status={message.StatusCode}, Content-Length={message.Content.Headers.ContentLength}");
                }

                request++;
            }
        }
    }

    class MyWrapperHttpHandler : DelegatingHandler
    {
        const int FailureRate = 50; // fail approx once every N requests

        public MyWrapperHttpHandler(HttpMessageHandler innerHandler) : base(innerHandler)
        {
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"{DateTime.Now}: {request.Method} {request.RequestUri}");

            Random r = new Random();
            bool fail = r.Next(FailureRate) == 0;

            if (fail)
            {
                Console.WriteLine("FAILING REQUEST");
                throw new Exception("request failed");
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
