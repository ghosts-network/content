from src.api import Api

class TestPublications(Api):
  def test_get_nonexistent_publication(self):
    resp = self.get_publication_by_id('nonexistent-id')

    assert resp.status_code == 404

  def test_get_publication_by_id(self):
    publication_resp = self.post_publication({'content': 'My first publication #awesome'})
    resp = self.get_publication_by_id(publication_resp.json()['id'])

    assert resp.status_code == 200

  def test_post_publication(self):
    resp = self.post_publication({'content': 'My first publication #awesome'})
    resp_body = resp.json()

    assert resp.status_code == 201

    assert 'id' in resp_body
    assert 'tags' in resp_body
    assert resp_body['tags'][0] == 'awesome'

  def test_empty_body_publication(self):
    resp = self.post_publication({})
    resp_body = resp.json()

    assert resp.status_code == 400

    resp_body = resp.json()
    assert 'Content' in resp_body['errors']
    
  def test_delete_publication(self):
    # create publication with 2 comments
    publication_resp = self.post_publication({'content': 'My first publication #awesome'})
    publication_id = publication_resp.json()['id']

    comment_1_resp = self.post_comment({'publicationId': publication_id, 'content': 'comment 1 for the first publication'})
    comment_1_id = comment_1_resp.json()['id']

    comment_2_resp = self.post_comment({'publicationId': publication_id, 'content': 'comment 2 for the first publication'})
    comment_2_id = comment_2_resp.json()['id']

    # delete publication
    delete_resp = self.delete_publication(publication_id)
    assert delete_resp.status_code == 200
    
    # ensure publication and comment deleted
    assert self.get_publication_by_id(publication_id).status_code == 404
    assert self.get_comment_by_id(comment_1_id).status_code == 404
    assert self.get_comment_by_id(comment_2_id).status_code == 404
