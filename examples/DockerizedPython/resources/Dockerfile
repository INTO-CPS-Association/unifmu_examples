FROM python:3.9

RUN pip install --upgrade pip
# backend dependencies
RUN pip install zmq protobuf

WORKDIR /resources
COPY deploy.py .
COPY backend.py .
COPY model.py .
COPY schemas schemas
