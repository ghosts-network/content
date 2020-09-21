import requests
import json

def test_get_nonexistent_publication():
  url = 'http://localhost:5300/publications/nonexistent-id'

  headers = {'Content-Type': 'application/json' }

  resp = requests.get(url, headers=headers)
  assert resp.status_code == 404

def test_get_publication_by_id():
  # create publication
  url = 'http://localhost:5300/publications'
  
  headers = {'Content-Type': 'application/json' }
  
  payload = {'content': 'My first publication #awesome'}

  resp = requests.post(url, headers=headers, data=json.dumps(payload,indent=4))

  # test
  resp_body = resp.json()
  url = 'http://localhost:5300/publications/' + resp_body['id']

  headers = {'Content-Type': 'application/json' }

  resp = requests.get(url, headers=headers)
  assert resp.status_code == 200