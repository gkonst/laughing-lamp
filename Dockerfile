FROM ubuntu
RUN apt update && apt install -y ca-certificates
COPY Build/Master/ /app/
WORKDIR /app 
ENTRYPOINT ["./MasterClient_qa.x86_64", "-nographics"] 
