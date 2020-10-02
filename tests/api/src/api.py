import requests
import json

class Api:
  def post_publication(self, body):
    url = 'http://localhost:5300/publications'
    headers = {'Content-Type': 'application/json' }

    return requests.post(url, headers=headers, data=json.dumps(body, indent=4))

  def get_publication_by_id(self, id):
    url = 'http://localhost:5300/publications/' + id
    headers = {'Content-Type': 'application/json' }

    return requests.get(url, headers=headers)
    
  def delete_publication(self, id):
    url = 'http://localhost:5300/publications/' + id
    headers = {'Content-Type': 'application/json' }
    
    return requests.delete(url, headers=headers)

  def post_comment(self, body):
    url = 'http://localhost:5300/comments'
    headers = {'Content-Type': 'application/json' }

    return requests.post(url, headers=headers, data=json.dumps(body, indent=4))

  def get_comment_by_id(self, id):
    url = 'http://localhost:5300/comments/' + id
    headers = {'Content-Type': 'application/json' }

    return requests.get(url, headers=headers)

  def get_comments_by_publication_id(self, publication_id):
    url = 'http://localhost:5300/comments/bypublication/' + publication_id
    headers = {'Content-Type': 'application/json' }

    return requests.get(url, headers=headers)

  def delete_comment(self, id):
    url = 'http://localhost:5300/comments/' + id
    headers = {'Content-Type': 'application/json' }
    
    return requests.delete(url, headers=headers)
