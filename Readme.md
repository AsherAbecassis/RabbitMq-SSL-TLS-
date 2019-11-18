# SSL in RabbitMQ

### SSL Certificates

To generate self-signed certificates we can use [tls-gen](https://github.com/michaelklishin/tls-gen):

```bash
git clone https://github.com/michaelklishin/tls-gen tls-gen
cd tls-gen/basic
make PASSWORD=random_pw
make verify
make info
ls -l ./result
cp result/ -R ../../certificates
cd ../..
```

To check the validity of the certificate:

`openssl x509 -enddate -noout -in ./tls-gen/basic/result/client_certificate.pem`

To verify the certificates and troubleshoot them we can use openssl:

```bash
# Create a server/listening service using the server_certificate
openssl s_server -accept 8443 -cert certificates/server_certificate.pem -key certificates/server_key.pem -CAfile certificates/ca_certificate.pem
# Test it with the client certificate
openssl s_client -connect localhost:8443 -cert certificates/client_certificate.pem -key certificates/client_key.pem -CAfile certificates/ca_certificate.pem
```

## Docker configuration

RabbitMQ is prepared to [auto-configure itself](https://github.com/CVTJNII/rabbitmq-1/blob/master/docker-entrypoint.sh) according to the environment variables/configuration provided.
An example configuration is as follows:

```yaml
version: "3"
services:

  rabbit:
    image: rabbitmq:3.7.11-management-alpine
    environment:
      - RABBITMQ_DEFAULT_USER=test
      - RABBITMQ_DEFAULT_PASS=test
      - RABBITMQ_SSL_CACERTFILE=/etc/ssl/rabbit/cacert.pem
      - RABBITMQ_SSL_CERTFILE=/etc/ssl/rabbit/cert.pem
      - RABBITMQ_SSL_FAIL_IF_NO_PEER_CERT=false
      - RABBITMQ_SSL_KEYFILE=/etc/ssl/rabbit/key.pem
      - RABBITMQ_SSL_VERIFY=verify_peer
    ports:
    - "5671:5671"
    volumes:
    - ./certificates/ca_certificate.pem:/etc/ssl/rabbit/cacert.pem
    - ./certificates/server_certificate.pem:/etc/ssl/rabbit/cert.pem
    - ./certificates/server_key.pem:/etc/ssl/rabbit/key.pem
```

You can now run the container with `docker-compose up -d` for example. (You can run it out of the box with the sample certificates present in the repository)

* rabbitmqctl in a TLS enabled RabbitMQ instance

When SSL/TLS is enabled the default tools will also require configuration:

```bash
cat "$RABBITMQ_SSL_CERTFILE" "$RABBITMQ_SSL_KEYFILE" > /tmp/combined.pem
chmod 0400 /tmp/combined.pem
export ERL_SSL_PATH="$(erl -eval 'io:format("~p", [code:lib_dir(ssl, ebin)]),halt().' -noshell)"
export RABBITMQ_SERVER_ADDITIONAL_ERL_ARGS="-pa '$ERL_SSL_PATH' -proto_dist inet_tls -ssl_dist_opt server_certfile /tmp/combined.pem -ssl_dist_opt server_secure_renegotiate true client_secure_renegotiate true"
export RABBITMQ_CTL_ERL_ARGS="$RABBITMQ_SERVER_ADDITIONAL_ERL_ARGS"
rabbitmqctl list_users
```

Alternatively you can use from outside the container:

`docker-compose exec rabbit docker-entrypoint.sh rabbitmqctl list_queues`

From inside the container

`docker-entrypoint.sh rabbitmqctl list_queues`

## Testing Python pub/sub

### (Optional) Create venv for python and install required packages to run the examples

```bash
virtualenv venv
source venv/bin/activate
pip install pika
```

You can test publishing/subscribing messages with `pub_test.py` and `sub_test.py`
