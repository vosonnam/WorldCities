const PROXY_CONFIG = [
  {
    context: [
      "/api",
    ],
    target: "http://localhost:5073",
    secure: false
  }
]

module.exports = PROXY_CONFIG;
