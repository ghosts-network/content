from src.api import Api

class TestComments(Api):
  def test_get_nonexistent_comment(self):
    resp = self.get_comment_by_id('nonexistent-id')

    assert resp.status_code == 404
  
  def test_get_comment_by_id(self):
    publication_resp = self.post_publication({'content': 'My first publication #awesome'})
    publication_id = publication_resp.json()['id']

    comment_resp = self.post_comment({'publicationId': publication_id, 'content': 'comment for the first publication'})
    comment_id = comment_resp.json()['id']

    resp = self.get_comment_by_id(comment_id)
    resp_body = resp.json()

    assert resp.status_code == 200

  def test_get_comments_by_publication(self):
    publication_resp = self.post_publication({'content': 'My first publication #awesome'})
    publication_id = publication_resp.json()['id']

    comment_resp = self.post_comment({'publicationId': publication_id, 'content': 'comment for the first publication'})
    comment_id = comment_resp.json()['id']
    
    resp = self.get_comments_by_publication_id(publication_id)
    resp_body = resp.json()

    assert resp.status_code == 200
    assert resp.headers['X-TotalCount'] == '1'

    assert len(resp_body) == 1
    assert resp_body[0]['id'] == comment_id
    assert resp_body[0]['publicationId'] == publication_id
    assert resp_body[0]['content'] == 'comment for the first publication'

  def test_get_comment_by_nonexistent_publication(self):
    resp = self.get_comments_by_publication_id('nonexistent-id')
    resp_body = resp.json()

    assert resp.status_code == 200
    assert resp.headers['X-TotalCount'] == '0'

    assert len(resp_body) == 0

  def test_post_comment(self):
    publication_resp = self.post_publication({'content': 'My first publication #awesome'})
    publication_id = publication_resp.json()['id']

    resp = self.post_comment({'publicationId': publication_id, 'content': 'comment for the first publication'})
    resp_body = resp.json()

    assert resp.status_code == 201
    assert 'id' in resp_body

  def test_post_comment_with_nonexistent_publication(self):
    resp = self.post_comment({'publicationId': 'nonexistent-id', 'content': 'comment for the first publication'})
    resp_body = resp.json()

    assert resp.status_code == 400

  def test_empty_body_comment(self):
    resp = self.post_comment({})
    resp_body = resp.json()

    assert resp.status_code == 400
    assert 'Content' in resp_body['errors']
    
  def test_delete_publication(self):
    # create publication with comment
    publication_resp = self.post_publication({'content': 'My first publication #awesome'})
    publication_id = publication_resp.json()['id']

    comment_resp = self.post_comment({'publicationId': publication_id, 'content': 'comment 2 for the first publication'})
    comment_id = comment_resp.json()['id']

    # delete comment
    delete_resp = self.delete_comment(comment_id)
    assert delete_resp.status_code == 200
    
    # ensure comment deleted
    assert self.get_comment_by_id(comment_id).status_code == 404
