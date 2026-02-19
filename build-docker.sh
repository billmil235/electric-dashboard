#!/bin/bash

echo "Building API image..."
docker build -f ElectricDashboard.Api/Dockerfile --target api-final -t electricdashboard-api .

echo "Building Frontend image..."
docker build -f ElectricDashboard.Api/Dockerfile --target frontend-final -t electricdashboard-frontend .

echo "Build complete!"
echo "Run with:"
echo "  docker run -p 5281:5281 electricdashboard-api"
echo "  docker run -p 4200:4200 electricdashboard-frontend"
