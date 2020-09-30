import requests
import json

def test_post_comment():
  url = 'http://localhost:5300/publications'
  
  headers = {'Content-Type': 'application/json' }
  
  payload = {'content': 'My first publication #awesome'}

  resp = requests.post(url, headers=headers, data=json.dumps(payload,indent=4))
  publicationId = resp.json()['id']

  url = 'http://localhost:5300/comments'

  headers = {'Content-Type': 'application/json' }

  payload = {
    'publicationId': publicationId,
    'content': 'comment for the first publication'
  }

  resp = requests.post(url, headers=headers, data=json.dumps(payload,indent=4))
  assert resp.status_code == 201

  resp_body = resp.json()
  assert 'id' in resp_body

def test_empty_body_comment():
  url = 'http://localhost:5300/comments/'
  
  headers = {'Content-Type': 'application/json' }
  
  payload = {}

  resp = requests.post(url, headers=headers, data=json.dumps(payload,indent=4))
  assert resp.status_code == 400

  resp_body = resp.json()
  assert 'Content' in resp_body['errors']
