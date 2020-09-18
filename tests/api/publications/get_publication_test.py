import requests
import json

def test_get_nonexistent_publication():
  url = 'http://localhost:5300/publications/nonexistent-id'

  headers = {'Content-Type': 'application/json' }

  resp = requests.get(url, headers=headers)
  assert resp.status_code == 404
